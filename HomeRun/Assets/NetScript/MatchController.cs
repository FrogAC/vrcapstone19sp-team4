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
        private PlayerType m_playerType = PlayerType.Batter;

        // Text to display when the match will start or finish
        [SerializeField] private Text m_timerText = null;

        // the camera is moved between the idle position and the assigned court position
        [SerializeField] private GameObject m_player = null;

        // where the camera will be when not in a match
        [SerializeField] private Transform m_idleAreaTransform = null;

        // this should equal the maximum number of players configured on the Oculus Dashboard
        [SerializeField] private PlayerArea[] m_playerAreas = new PlayerArea[2];

        // seconds to wait to coordinate P2P setup with other match players before starting
        [SerializeField] private uint MATCH_WARMUP_TIME = 10;

        // seconds for the match
        [SerializeField] private uint MATCH_TIME = 999;

        // the current state of the match controller
        private State m_currentState;

        // transition time for states that automatically transition to the next state,
        // for example ending the match when the timer expires
        private float m_nextStateTransitionTime;

        // the court the local player was assigned to
        private int m_localSlot;

        public PlayerType PlayerType
        {
            get { return m_playerType; }
            set
            {
                m_playerType = value;
                PlatformManager.Instance.SetTransformActiveFromType(value);
            }
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
                        Assert.AreEqual(oldState, State.NONE);
                        PlatformManager.TransitionToState(PlatformManager.State.MATCH_TRANSITION);
                        break;

                    case State.WAITING_TO_SETUP_MATCH:
                        Assert.AreEqual(oldState, State.WAITING_FOR_MATCH);
                        m_nextStateTransitionTime = Time.time + MATCH_WARMUP_TIME;
                        break;

                    case State.PLAYING_MATCH:
                        Assert.AreEqual(oldState, State.WAITING_TO_SETUP_MATCH);
                        PlatformManager.TransitionToState(PlatformManager.State.PLAYING_A_NETWORKED_MATCH);
                        m_nextStateTransitionTime = Time.time + MATCH_TIME;
                        Debug.Log("Match Time Until" + m_nextStateTransitionTime);
                        break;
                }
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
                        m_timerText.text = string.Format("{0:0}", Mathf.Ceil(Time.time - MatchStartTime));
                        break;
                    case State.PLAYING_MATCH:
                        var delta = m_nextStateTransitionTime - Time.time;
                        delta = 50.0f;
                        m_timerText.text = string.Format("{0:#0}:{1:#00}.{2:00}",
                            Mathf.Floor(delta / 60),
                            Mathf.Floor(delta) % 60,
                            Mathf.Floor(delta * 100) % 100);
                        break;
                }
            }
        }

        #endregion

        #region Player Setup/Teardown

        void SetupForIdle()
        {
            for (int i = 0; i < m_playerAreas.Length; i++)
            {
                m_playerAreas[i].SetupForPlayer<AIPlayer>("* AI *");
            }
        }

        Player MatchPlayerAddedCallback(int slot, User user)
        {
            Player player = null;

            if (m_currentState == State.WAITING_TO_SETUP_MATCH && slot < m_playerAreas.Length)
            {
                if (user.ID == PlatformManager.MyID)
                {
                    var localPlayer = m_playerAreas[slot].SetupForPlayer<LocalPlayer>(user.OculusID);
                    MoveCameraToMatchPosition();
                    player = localPlayer;
                    m_localSlot = slot;
                }
                else
                {
                    var remotePlayer = m_playerAreas[slot].SetupForPlayer<RemotePlayer>(user.OculusID);
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
        }

        void MoveCameraToMatchPosition()
        {
            foreach (var playerArea in m_playerAreas)
            {
                var player = playerArea.GetComponentInChildren<LocalPlayer>();
                if (player)
                {
                    m_player.transform.SetParent(player.transform, false);
                    m_player.transform.localPosition = Vector3.zero;
                    break;
                }
            }
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
