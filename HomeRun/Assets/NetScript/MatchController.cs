namespace HomeRun.Net
{
    using UnityEngine;
    using UnityEngine.Assertions;
    using UnityEngine.UI;
    using System.Collections.Generic;
    using Oculus.Platform.Models;

    // This class coordinates playing matches.  It mediates being idle
    // and entering a practice or online game match.
    public class MatchController : MonoBehaviour
    {
        // meant to be single instance
        private static PlayerType m_playerType = PlayerType.None;

        // Text to display when the match will start or finish
        [SerializeField] private Transform m_startObjects;
        [SerializeField] private Text m_timerText = null;
        [SerializeField] private Text m_timerText1 = null;

        // the camera is moved between the idle position and the assigned court position
        [SerializeField] private GameObject m_player = null;

        // where the camera will be when not in a match
        [SerializeField] private Transform m_idleAreaTransform = null;

        // this should equal the maximum number of players configured on the Oculus Dashboard
        // 0 = Batter, 1 = Pitcher
        [SerializeField] private PlayerArea[] m_playerAreas = new PlayerArea[2];
        // seconds to wait to coordinate P2P setup with other match players before starting
        [SerializeField] private uint MATCH_WARMUP_TIME = 15;

        // seconds for the match
        [SerializeField] private uint MATCH_TIME = 360;

        // the current state of the match controller
        private State m_currentState;

        // transition time for states that automatically transition to the next state,
        // for example ending the match when the timer expires
        private float m_nextStateTransitionTime;

        // the court the local player was assigned to
        private int m_localSlot;        
        private static MatchController s_instance;

        public static MatchController Instance
        {
            get { return s_instance; }
        }

        public static PlayerType PlayerType
        {
            get { return m_playerType; }
            set
            {
                m_playerType = value;
                PlatformManager.Instance.SetTransformActiveFromType(value);
            }
        }
        public PlayerArea[] PlayerAreas {
            get {return m_playerAreas;}
        }

        void Awake() {
            if (s_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            s_instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            PlatformManager.Matchmaking.EnqueueResultCallback = OnMatchFoundCallback;
            PlatformManager.Matchmaking.MatchPlayerAddedCallback = MatchPlayerAddedCallback;
            PlatformManager.P2P.StartTimeOfferCallback = StartTimeOfferCallback;

            TransitionToState(State.NONE);
        }

        void Update()
        {
            UpdateCheckForNextTimedTransition();
            UpdateMatchTimer();
        }

        public float MatchStartTime
        {
            get
            {
                switch (m_currentState)
                {
                    case State.WAITING_TO_SETUP_MATCH:
                        return m_nextStateTransitionTime;

                    default: return 0;
                }
            }
            private set { m_nextStateTransitionTime = value; }
        }

        #region State Management

        private enum State
        {
            UNKNOWN,

            // no current match, waiting for the local user to select something
            NONE,

            // selecting Player Online and waiting for the Matchmaking service to find and create a
            // match and join the assigned match room
            WAITING_FOR_MATCH,

            // match room is joined, waiting to coordinate with the other players
            WAITING_TO_SETUP_MATCH,

            // playing a competative match against other players
            PLAYING_MATCH
        }

        void TransitionToState(State newState)
        {
            Debug.LogFormat("MatchController State {0} -> {1}", m_currentState, newState);

            if (m_currentState != newState)
            {
                var oldState = m_currentState;
                m_currentState = newState;

                // state transition logic
                switch (newState)
                {
                    case State.NONE:
                        SetupForIdle();
                        MoveCameraToIdlePosition();
                        PlatformManager.TransitionToState(PlatformManager.State.WAITING_TO_PRACTICE_OR_MATCHMAKE);
                        break;

                    case State.WAITING_FOR_MATCH:
                        SetAllAreaText("Waiting...");
                        m_startObjects.gameObject.SetActive(false);
                        Assert.AreEqual(oldState, State.NONE);
                        PlatformManager.TransitionToState(PlatformManager.State.MATCH_TRANSITION);
                        break;

                    case State.WAITING_TO_SETUP_MATCH:
                        if (MatchController.m_playerType == PlayerType.Pitcher) NetStrikeZone.strikezone.SetVisibal(true);
                        Assert.AreEqual(oldState, State.WAITING_FOR_MATCH);
                        m_nextStateTransitionTime = Time.time + MATCH_WARMUP_TIME;
                        break;

                    case State.PLAYING_MATCH:
                        SetAllAreaText("");
                        Assert.AreEqual(oldState, State.WAITING_TO_SETUP_MATCH);
                        PlatformManager.TransitionToState(PlatformManager.State.PLAYING_A_NETWORKED_MATCH);
                        m_nextStateTransitionTime = Time.time + MATCH_TIME;
                        Debug.Log("Match Time Until" + m_nextStateTransitionTime);
                        break;
                }
            }
        }

        void SetAllAreaText(string text) {
            foreach (var pa in m_playerAreas) {
                pa.NameText.text = text;
            }
        }

        void UpdateCheckForNextTimedTransition()
        {
            if (m_currentState != State.NONE && Time.time >= m_nextStateTransitionTime)
            {
                switch (m_currentState)
                {

                    case State.WAITING_TO_SETUP_MATCH:
                        TransitionToState(State.PLAYING_MATCH);
                        break;

                    case State.PLAYING_MATCH:
                        TransitionToState(State.NONE);
                        break;
                }
            }
        }

        void UpdateMatchTimer()
        {
            if (Time.time <= m_nextStateTransitionTime)
            {
                switch (m_currentState)
                {
                    case State.WAITING_TO_SETUP_MATCH:
                        m_timerText.text = "Warm-UP: " + string.Format("{0:0}", Mathf.Ceil(Time.time - MatchStartTime));
                        m_timerText1.text = m_timerText.text;
                        break;
                    case State.PLAYING_MATCH:
                        var delta = m_nextStateTransitionTime - Time.time;
                        m_timerText.text = "Time: " + string.Format("{0:#0}:{1:#00}",
                            Mathf.Floor(delta / 60),
                            Mathf.Floor(delta) % 60);
                        m_timerText1.text = m_timerText.text;
                        break;
                }
            }
        }

        #endregion

        #region Player Setup/Teardown

        void SetupForIdle()
        {
            NetStrikeZone.strikezone.SetVisibal(false);
            NetStrikeZone.strikezone.SetMotion(false);
            SetAllAreaText("");
            PlayerType = PlayerType.None;  // do not reset transforms
            m_startObjects.gameObject.SetActive(true);
            MoveCameraToIdlePosition();
        }

        Player MatchPlayerAddedCallback(int slot, User user)
        {
            Player player = null;

            // slot used only for verification at this point
            if (m_currentState == State.WAITING_TO_SETUP_MATCH && slot >= 0)
            {
                if (user.ID == PlatformManager.MyID)
                {
                    var localPlayer = m_playerAreas[(int)m_playerType].SetupForPlayer<LocalPlayer>(user.OculusID);
                    MoveCameraToMatchPosition();
                    player = localPlayer;
                    m_localSlot = slot;
                }
                else
                {
                    var remotePlayer = m_playerAreas[1-(int)m_playerType].SetupForPlayer<RemotePlayer>(user.OculusID);
                    remotePlayer.User = user;
                    player = remotePlayer;
                }
            }

            return player;
        }

        #endregion

        #region Main Camera Movement

        void MoveCameraToIdlePosition()
        {
            m_player.transform.SetParent(m_idleAreaTransform, false);
            m_player.transform.localPosition = Vector3.zero;
            m_player.transform.rotation = m_idleAreaTransform.rotation;
        }

        void MoveCameraToMatchPosition()
        {
            // batter goto 0, pitcher 1
            var holder = m_playerAreas[(int)m_playerType % 2].PlayerHolder;
            m_player.transform.SetParent(holder, false);
            m_player.transform.localPosition = Vector3.zero;
            m_player.transform.rotation = holder.rotation;
        }

        #endregion

        #region Match Initiation

        public void PlayOnlineOrCancel()
        {
            Debug.Log("Play online or Cancel");

            if (m_currentState == State.NONE)
            {
                PlatformManager.Matchmaking.QueueForMatch();
                TransitionToState(State.WAITING_FOR_MATCH);
            }
            else if (m_currentState == State.WAITING_FOR_MATCH)
            {
                PlatformManager.Matchmaking.LeaveQueue();
                TransitionToState(State.NONE);
            }
        }


        // notification from the Matchmaking service if we succeeded in finding an online match
        void OnMatchFoundCallback(bool success)
        {
            if (success)
            {
                TransitionToState(State.WAITING_TO_SETUP_MATCH);
            }
            else
            {
                TransitionToState(State.NONE);
            }
        }

        // handle an offer from a remote player for a new match start time
        float StartTimeOfferCallback(float remoteTime)
        {
            if (m_currentState == State.WAITING_TO_SETUP_MATCH)
            {
                // if the remote start time is later use that, as long as it's not horribly wrong
                if (remoteTime > MatchStartTime && (remoteTime - 60) < MatchStartTime)
                {
                    Debug.Log("Moving Start time by " + (remoteTime - MatchStartTime));
                    MatchStartTime = remoteTime;
                }
            }
            return MatchStartTime;
        }

        #endregion

    }
}
