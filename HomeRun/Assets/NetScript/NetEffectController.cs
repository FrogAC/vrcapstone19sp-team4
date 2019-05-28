using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HomeRun.Game;

namespace HomeRun.Net
{
    public class NetEffectController : MonoBehaviour
    {
        private static NetEffectController s_instance;
        public static NetEffectController Instance
        {
            get { return s_instance; }
        }

        [Header("0: fast, 1: curve, 2: spiral")]
        [SerializeField] private GameObject[] m_Bathit_Prefabs;
        [SerializeField] private GameObject[] m_Strikehit_Prefabs;
        private ParticleSystem[] m_BathitParticles = new ParticleSystem[3];
        private ParticleSystem[] m_StrikerhitParticles = new ParticleSystem[3];


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

        public void PlayBatHitEffect(Vector3 pos, BallType type)
        {
            int idx = (int) type;
            if (!m_BathitParticles[idx])
            {
                m_BathitParticles[idx] = Instantiate(m_Bathit_Prefabs[idx], transform.position, transform.rotation).GetComponentInChildren<ParticleSystem>();
            }
            m_BathitParticles[idx].transform.position = pos;
            m_BathitParticles[idx].Play();
        }

        public void PlayStrikeZoneHitEffect(Vector3 pos, BallType type)
        {
            int idx = (int) type;
            if (!m_StrikerhitParticles[idx])
            {
                m_StrikerhitParticles[idx] = Instantiate(m_Strikehit_Prefabs[idx], transform.position, transform.rotation).GetComponentInChildren<ParticleSystem>();
            }
            m_StrikerhitParticles[idx].transform.position = pos;
            m_StrikerhitParticles[idx].Play();
        }
    }
}