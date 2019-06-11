namespace HomeRun.Net
{
    using UnityEngine;
    using UnityEngine.Assertions;
    using UnityEngine.UI;
    using System.Collections;
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
        [SerializeField] private GameObject m_ScoreHolder = null;
        [SerializeField] private GameObject m_ScoreHolder1 = null;
        [SerializeField] private Text m_ScoreBoard = null;
        [SerializeField] private Text m_ScoreBoard1 = null;

        private int m_myScore = 0;
        private int m_opScore = 0;


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
        public PlayerArea[] PlayerAreas
        {
            get { return m_playerAreas; }
        }

        void Awake()
        {
            if (s_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            s_instance = this;
            GlobalSettings.UseNetwork = true;
            GlobalSettings.Selectable = true;
            //DontDestroyOnLoad(gameObject);
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
                        if (m_playerType == PlayerType.Batter)
                        {
                            m_ScoreHolder.SetActive(true);
                            m_ScoreHolder1.SetActive(false);
                        }
                        else
                        {
                            m_ScoreHolder.SetActive(false);
                            m_ScoreHolder1.SetActive(true);
                        }
                        m_myScore = 0;
                        m_opScore = 0;
                        Assert.AreEqual(oldState, State.WAITING_TO_SETUP_MATCH);
                        PlatformManager.TransitionToState(PlatformManager.State.PLAYING_A_NETWORKED_MATCH);
                        m_nextStateTransitionTime = Time.time + MATCH_TIME;
                        Debug.Log("Match Time Until" + m_nextStateTransitionTime);
                        break;
                }
            }
        }

        void SetAllAreaText(string text)
        {
            foreach (var pa in m_playerAreas)
            {
                pa.NameText.text = text;
            }
        }

        /**
            Miss : Batter + 1
            Strike: Pitcher + 4
            Homerun: Batter + 5
            Hit: Batter + 1
         */
        public void UpdateScore(int batterChange, int pitcherChange)
        {
            m_myScore += (m_playerType == PlayerType.Batter) ? batterChange : pitcherChange;
            m_opScore += (m_playerType == PlayerType.Batter) ? pitcherChange : batterChange;
            if (m_myScore < 0) m_myScore = 0;
            if (m_opScore < 0) m_opScore = 0;
            m_ScoreBoard.text = string.Format("{0:#00} - {1:#00}",
                                m_myScore, m_opScore);
            m_ScoreBoard1.text = m_ScoreBoard.text;
            StartCoroutine(RotateOutBack((m_playerType == PlayerType.Batter) ? m_ScoreHolder.transform : m_ScoreHolder1.transform, 1.5f));
        }

        IEnumerator RotateOutBack(Transform transform, float time)
        {
            float remain = time;
            while (remain > 0.0f)
            {
                remain -= Time.deltaTime;
                float t = 1 - remain / time;
                t = EaseOutElasticBack(t);
                transform.localRotation = Quaternion.AngleAxis(t, Vector3.up);
                yield return null;
            }
        }

        public static float EaseOutElasticBack(float value)
        {
            float start = 0.0f;
            float end = 360.0f;
            float d = 1f;
            float p = d * .3f;
            float s;
            float a = 0;

            if (value == 0) return start;

            if ((value /= d) == 1) return start + end;

            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p * 0.25f;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }

            return (a * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) + end + start);
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
            m_ScoreHolder.SetActive(false);
            m_ScoreHolder1.SetActive(false);

            m_timerText.text = "";
            m_timerText1.text = "";
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
                    var remotePlayer = m_playerAreas[1 - (int)m_playerType].SetupForPlayer<RemotePlayer>(user.OculusID);
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
            m_player.transform.SetParent(m_idleAreaTransform, true);
            m_player.transform.localPosition = Vector3.zero;
            m_player.transform.rotation = m_idleAreaTransform.rotation;
        }

        void MoveCameraToMatchPosition()
        {
            // batter goto 0, pitcher 1
            var holder = m_playerAreas[(int)m_playerType % 2].PlayerHolder;
            m_player.transform.SetParent(holder, true);
            Invoke("ResetLocalPosition", 4);
            Invoke("ResetLocalPosition", 8);
            m_player.transform.rotation = holder.rotation;
            m_player.GetComponent<CharacterController>().SimpleMove(Vector3.zero);
        }

        void ResetLocalPosition() {
            Debug.Log("ResetLocalPosition");
            m_player.transform.localPosition = Vector3.zero;
            m_player.GetComponent<CharacterController>().SimpleMove(Vector3.zero);
            m_myScore = 0;
            m_opScore = 0;
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
        void OnDestory() {
            GlobalSettings.UseNetwork = false;
        }

    }


}
