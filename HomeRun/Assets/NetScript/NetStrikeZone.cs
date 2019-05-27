using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HomeRun.Net
{
    /* An addition to Strikezone. Controls movement */
    public class NetStrikeZone : MonoBehaviour
    {
        [SerializeField] private Vector3 m_moveVec = new Vector3(1, 0, 0);
        [SerializeField] private GameObject m_visual;
        [SerializeField] private float m_timerate = 1.0f;
        public static NetStrikeZone strikezone;
        private bool m_enableMove = true;
        private Vector3 origin;
        private void Awake()
        {
            if (strikezone == null)
            {
                strikezone = this;
                origin = transform.position;
            }
        }

        void Update()
        {
            if (!m_enableMove) return;
            var t = (Time.time % m_timerate) / m_timerate;  // [0,1]
            t = t < .5f ? 2 * t * t : -1 + (4 - 2 * t) * t;  // easeInOut
            t = (t - 0.5f) * 2.0f; 
            if ((Time.time % (2*m_timerate)) / (2*m_timerate) > .5f) t = -t; 
            transform.position = origin + t * m_moveVec;
        }

        public void SetVisual(bool b)
        {
            m_visual.SetActive(b);
        }

        public void SetMotion(bool b)
        {
            m_enableMove = b;
        }

        // Start is called before the first frame update
        void Start()
        {

        }
    }
}