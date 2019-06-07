namespace HomeRun.Game
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using HomeRun.Net;
    using TMPro;

    /// 0: fast, 1: curve, 2: spiral
    public enum BallType : int
    {
        FastBall = 0,
        CurveBall = 1,
        SpiralBall = 2,
        LightningBall = 3,
        ControlBall = 4

    }

    public class BallSelector : MonoBehaviour
    {
        [SerializeField] Selection[] selections = new Selection[0];
        int currentIndex = 0;

        [Space]

        [SerializeField] TextMeshProUGUI ballNameText;
        [SerializeField] Transform spawnPoint;

        public Transform SpawnPoint
        {
            get { return spawnPoint; }
        }

        GameObject spawnedPrefab = null;
        OVRGrabbable spawnedGrabbable;
        [SerializeField] OVRInput.Button selectNext = OVRInput.Button.DpadLeft;
        [SerializeField] OVRInput.Button selectPrev = OVRInput.Button.DpadRight;

        /* Multiplayer Settings */
        [Header("Multiplayer")]

        private float m_nextSelectableInterval = 9.0f;
        private float m_nextSelectableTime = -1.0f;

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
        public GameObject CreateBall(BallType ballType)
        {
            GameObject ball = Instantiate(selections[(int)ballType].prefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
            //ball.GetComponent<Rigidbody>().isKinematic = true;
            return ball;
        }

        void UpdateSelection()
        {

            if (spawnedPrefab != null)
            {
                Destroy(spawnedPrefab.gameObject);
            }
            else if (GlobalSettings.UseNetwork)
            {
                // adding time limit when multiplayer match
                if (!GlobalSettings.Selectable)
                {
                    if (ballNameText.text == "Not Yet")
                        ballNameText.text = "Not Yet:(";
                    else if (ballNameText.text == "Not Yet:(")
                        ballNameText.text = "No :(";
                    else if (ballNameText.text == "No :(")
                        ballNameText.text = "PATIENT";
                    else if (ballNameText.text != "PATIENT")
                        ballNameText.text = "Not Yet";
                    return;
                }
                else
                {
                    NetStrikeZone.strikezone.SetMotion(true);
                    GlobalSettings.Selectable = false;  // disable until ball lands
                    //m_nextSelectableTime = Time.time + m_nextSelectableInterval;
                }
            }

            spawnedPrefab = Instantiate(selections[currentIndex].prefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
            var tb = spawnedPrefab.GetComponent<ThrownBall>();
            spawnedGrabbable = tb;
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