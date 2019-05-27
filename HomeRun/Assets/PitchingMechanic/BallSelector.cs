namespace HomeRun.Game
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using HomeRun.Net;
    using TMPro;

    public enum BallType : int
    {
        FastBall = 0,
        CurveBall = 1,
        SpiralBall = 2
    }

    public class BallSelector : MonoBehaviour
    {
        [SerializeField] Selection[] selections = new Selection[0];
        int currentIndex = 0;

        [Space]

        [SerializeField] TextMeshProUGUI ballNameText;
        [SerializeField] Transform spawnPoint;

        public Transform SpawnPoint {
            get {return spawnPoint;}
        }

        GameObject spawnedPrefab = null;
        OVRGrabbable spawnedGrabbable;
        [SerializeField] OVRInput.Button selectNext = OVRInput.Button.DpadLeft;
        [SerializeField] OVRInput.Button selectPrev = OVRInput.Button.DpadRight;

        /* Multiplayer Settings */
        [Header("Multiplayer")]

        [SerializeField] private float m_nextSelectableInterval = 3.0f;
        private float m_nextSelectableTime = float.MaxValue;


        // Start is called before the first frame update
        void Start()
        {
            UpdateSelection();
        }

        private void Update()
        {
            if (OVRInput.GetDown(selectNext))
            {
                NextSelection();
            }
            else if (OVRInput.GetDown(selectPrev))
            {
                PrevSelection();
            }

            if (spawnedGrabbable != null)
            {
                if (spawnedGrabbable.isGrabbed)
                {
                    spawnedPrefab.GetComponent<Rigidbody>().isKinematic = false;
                    spawnedPrefab.transform.SetParent(null);

                    spawnedPrefab = null;
                    spawnedGrabbable = null;
                    UpdateBallNameText();
                }
            }
        }

        // Used for Create remote Ball
        public GameObject CreateBall(BallType ballType) {
            GameObject ball = Instantiate(selections[(int)ballType].prefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
            //ball.GetComponent<Rigidbody>().isKinematic = true;
            return ball;
        }

        void UpdateSelection()
        {
            if (GlobalSettings.UseNetwork && PlatformManager.CurrentState == PlatformManager.State.PLAYING_A_NETWORKED_MATCH ) {
                if (Time.time < m_nextSelectableTime) {
                    ballNameText.text = "Not Yet:(";
                    return;
                } else {
                    NetStrikeZone.strikezone.SetMotion(true);
                    m_nextSelectableTime = Time.time + m_nextSelectableInterval;
                }
            }

            if (spawnedPrefab != null)
            {
                Destroy(spawnedPrefab.gameObject);
            }

            spawnedPrefab = Instantiate(selections[currentIndex].prefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
            spawnedGrabbable = spawnedPrefab.GetComponent<ThrownBall>();
            spawnedPrefab.GetComponent<Rigidbody>().isKinematic = true;

            if (GlobalSettings.UseNetwork && PlatformManager.CurrentState == PlatformManager.State.PLAYING_A_NETWORKED_MATCH)
            {
                PlatformManager.P2P.AddNetworkBall(spawnedPrefab, selections[currentIndex].type);
            }
            UpdateBallNameText();
        }

        void UpdateBallNameText()
        {
            if (ballNameText != null)
            {
                if (spawnedPrefab != null)
                {
                    ballNameText.text = selections[currentIndex].name;
                }
                else
                {
                    ballNameText.text = "";
                }

            }
        }

        public void NextSelection()
        {
            if (spawnedPrefab != null)
            {
                currentIndex = (currentIndex + 1) % selections.Length;
            }
            UpdateSelection();
        }

        public void PrevSelection()
        {
            if (spawnedPrefab != null)
            {
                currentIndex = ((currentIndex - 1) + selections.Length) % selections.Length;
            }
            UpdateSelection();
        }


        [System.Serializable]
        public class Selection
        {
            public string name;
            public BallType type;
            public GameObject prefab;
        }
    }
}