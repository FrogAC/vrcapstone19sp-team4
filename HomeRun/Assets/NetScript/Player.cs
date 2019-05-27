namespace HomeRun.Net
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections.Generic;
    using HomeRun.Game;

    public enum PlayerType : int
    {
        Batter = 0,
        Pitcher = 1,
        None = 3
    }

    // The base Player component manages the balls that are in play.  Besides spawning new balls,
    // old balls are destroyed when too many are around or the Player object itself is destroyed.
    public abstract class Player : MonoBehaviour
    {

        // maximum number of balls allowed at a time
        public const uint MAX_BALLS = 6;

        // the initial force to impart when shooting a ball
        private const float INITIAL_FORCE = 870f;

        // delay time before a new ball will spawn.
        private const float RESPAWN_SECONDS = 2.0f;

        // cached reference to the Text component to render the score
        private Text m_scoreUI;

        // prefab for the GameObject representing a ball
        private GameObject m_ballPrefab;

        // Pitcher
        private BallSelector m_ballSelector;

        protected BallSelector BallSelector
        {
            set { m_ballSelector = value; }
        }

        // queue of active balls for the player to make sure too many arent in play
        private Queue<GameObject> m_balls = new Queue<GameObject>();

        void Start()
        {
            m_scoreUI = transform.parent.GetComponentInChildren<Text>();
            m_scoreUI.text = "0";
        }

        public GameObject CreateBall(BallType ballType)
        {
            if (m_balls.Count >= MAX_BALLS)
            {
                Destroy(m_balls.Dequeue());
            }
            var ball = m_ballSelector.CreateBall(ballType);
            m_balls.Enqueue(ball);

            // ball.transform.position = m_ballSelector.SpawnPoint.position;
            // ball.transform.SetParent(m_ballSelector.SpawnPoint, true);
            return ball;
        }

        // protected GameObject ShootBall()
        // {
        // 	GameObject ball = m_heldBall;
        // 	m_heldBall = null;

        // 	ball.GetComponent<Rigidbody>().useGravity = true;
        // 	ball.GetComponent<Rigidbody>().detectCollisions = true;
        // 	ball.GetComponent<Rigidbody>().AddForce(m_ballEjector.transform.forward * INITIAL_FORCE, ForceMode.Acceleration);
        // 	ball.transform.SetParent(transform.parent, true);

        // 	m_nextSpawnTime = Time.time + RESPAWN_SECONDS;
        // 	return ball;
        // }

        void OnDestroy()
        {
            foreach (var ball in m_balls)
            {
                Destroy(ball);
            }
        }
    }
}
