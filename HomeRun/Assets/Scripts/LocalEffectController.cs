using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HomeRun.Game;

namespace HomeRun.Game
{

    /**
        This also controls Scores of multiplayer
     */
    public class LocalEffectController : MonoBehaviour
    {
        private static LocalEffectController s_instance;
        public static LocalEffectController Instance
        {
            get { return s_instance; }
        }

        [Header("0: fast, 1: curve, 2: spiral")]
        [SerializeField] private GameObject[] m_Bathit_Prefabs;
        [SerializeField] private GameObject m_GroundHit_Prefab;
        [SerializeField] private GameObject m_StrikeMisstext_Prefab;
        [SerializeField] private GameObject m_Strikehit_Prefabs;
        [SerializeField] private GameObject m_Striketext_Prefabs;
        [SerializeField] private GameObject m_Homerunhit_Prefab;
        [SerializeField] private GameObject m_Homeruntext_Prefab;
        private ParticleSystem m_HomerunhitParticle;
        private Transform m_HomerunText;
        private Transform m_StrikeText;
        private Transform m_StrikeMisstext;

        private ParticleSystem[] m_BathitParticles = new ParticleSystem[3];
        private ParticleSystem m_GroundHitParticle;
        private ParticleSystem m_StrikerhitParticles;


        private Transform m_player;


        void Awake()
        {
            if (s_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            m_player = GameObject.FindGameObjectWithTag("Player").transform;
            s_instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void PlayBatHitEffect(Vector3 pos, BallType type)
        {
            int idx = (int)type;
            if (!m_BathitParticles[idx])
            {
                m_BathitParticles[idx] = Instantiate(m_Bathit_Prefabs[idx], transform.position, transform.rotation).GetComponentInChildren<ParticleSystem>();
            }

            m_BathitParticles[idx].transform.position = pos;
            m_BathitParticles[idx].Play();
        }
        public void PlayGroundHitEffect(Vector3 pos)
        {
            if (!m_GroundHitParticle)
            {
                m_GroundHitParticle = Instantiate(m_GroundHit_Prefab, transform.position, transform.rotation).GetComponentInChildren<ParticleSystem>();
            }
            m_GroundHitParticle.transform.position = pos;
            m_GroundHitParticle.Play();
        }

        public void PlayStrikeZoneHitEffect(Vector3 pos)
        {
            if (!m_StrikerhitParticles)
            {
                m_StrikerhitParticles = Instantiate(m_Strikehit_Prefabs, transform.position, transform.rotation).GetComponentInChildren<ParticleSystem>();
            }
            if (!m_StrikeText)
            {
                m_StrikeText = Instantiate(m_Striketext_Prefabs, transform.position, transform.rotation).transform;
            }

            m_StrikerhitParticles.transform.position = pos;
            m_StrikerhitParticles.Play();

            Vector3 dir = (m_player.position - pos).normalized;
            m_StrikeText.position = pos;
            m_StrikeText.rotation = Quaternion.LookRotation(dir, Vector3.up);
            StartCoroutine(Shrink(m_StrikeText, Vector3.zero, Vector3.one, 1.5f));
        }

        public void PlayStrikeZoneMissEffect(Vector3 pos)
        {
            if (!m_StrikeMisstext)
            {
                m_StrikeMisstext = Instantiate(m_StrikeMisstext_Prefab, transform.position, transform.rotation).transform;
            }


            Vector3 dir = (m_player.position - pos).normalized;
            m_StrikeMisstext.position = pos;
            m_StrikeMisstext.rotation = Quaternion.LookRotation(dir, Vector3.up);
            StartCoroutine(Shrink(m_StrikeMisstext, Vector3.zero, Vector3.one, 1.5f));
        }

        // explosion and fly toward player?
        public void PlayHomerunEffect(Vector3 pos)
        {
            if (!m_HomerunhitParticle)
                m_HomerunhitParticle = Instantiate(m_Homerunhit_Prefab, transform.position, transform.rotation).GetComponentInChildren<ParticleSystem>();
            m_HomerunhitParticle.transform.position = pos;
            if (!m_HomerunText)
            {
                m_HomerunText = Instantiate(m_Homeruntext_Prefab, transform.position, transform.rotation).transform;
                
            }

            StopAllCoroutines();
            


            m_HomerunhitParticle.Play();

            Vector3 dir = (m_player.position - pos).normalized;
            m_HomerunText.position = pos;
            m_HomerunText.rotation = Quaternion.LookRotation(dir, Vector3.up);
            foreach (Rigidbody rb in m_HomerunText.GetComponentsInChildren<Rigidbody>())
            {
                rb.velocity = Vector3.zero;
                rb.transform.localPosition = Vector3.zero;
                rb.AddForce((m_player.position - pos) * 8f, ForceMode.Force);
                //rb.AddExplosionForce(300.0f, pos - dir * 15f, 80.0f, 10.0f, ForceMode.Acceleration);
                StartCoroutine(Shrink(rb.transform, Vector3.zero, Vector3.one, 3.5f));
            }
        }


        // actually not shrink
        IEnumerator Shrink(Transform transform, Vector3 start, Vector3 end, float time)
        {
            float remain = time;
            while (remain > 0.0f)
            {
                remain -= Time.deltaTime;
                float t = 1 - remain / time;
                t = EaseOutBack(t);
                transform.localScale = Vector3.Lerp(start, end, t);
                yield return null;
            }
            remain = time / 1.5f;
            while (remain > 0.0f)
            {
                remain -= Time.deltaTime;
                float t = remain / (time / 1.5f);
                t = EaseOutBack(t);
                transform.localScale = Vector3.Lerp(start, end, t);
                yield return null;
            }
        }



        public static float EaseOutBack(float value)
        {
            float start = 0.0f;
            float end = 1.0f;

            // float s = 1.70158f;
            // value = (value) - 1;
            // return end * ((value) * value * ((s + 1) * value + s) + 1) + start;

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


    }
}