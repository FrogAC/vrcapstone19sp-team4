using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HomeRun.Net
{
    public class NetEffectController : MonoBehaviour
    {
        private static NetEffectController s_instance;
        public static NetEffectController Instance
        {
            get { return s_instance; }
        }

        [SerializeField]
        private ParticleSystem m_batHitPS_Prefab;
        private ParticleSystem m_batHitPS;


        void Awake()
        {
            if (s_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            s_instance = this;
            DontDestroyOnLoad(gameObject);
        }



        public void PlayBatHitEffect(Vector3 pos)
        {
            if (!m_batHitPS)
            {
                m_batHitPS = Instantiate(m_batHitPS_Prefab, transform.position, transform.rotation);
                m_batHitPS.transform.SetParent(transform);
            }
            m_batHitPS.transform.position = pos;
            m_batHitPS.Play();
        }
    }
}