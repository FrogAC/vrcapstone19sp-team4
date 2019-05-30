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
        [SerializeField] private GameObject m_Homerunhit_Prefab;
        [SerializeField] private GameObject m_Homeruntext_Prefab;
        private ParticleSystem m_HomerunhitParticle;
        private Transform m_HomerunText;
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
            int idx = (int)type;
            if (!m_BathitParticles[idx])
            {
                m_BathitParticles[idx] = Instantiate(m_Bathit_Prefabs[idx], transform.position, transform.rotation).GetComponentInChildren<ParticleSystem>();
            }
            m_BathitParticles[idx].transform.position = pos;
            m_BathitParticles[idx].Play();
        }

        public void PlayStrikeZoneHitEffect(Vector3 pos, BallType type)
        {
            int idx = (int)type;
            if (!m_StrikerhitParticles[idx])
            {
                m_StrikerhitParticles[idx] = Instantiate(m_Strikehit_Prefabs[idx], transform.position, transform.rotation).GetComponentInChildren<ParticleSystem>();
            }
            m_StrikerhitParticles[idx].transform.position = pos;
            m_StrikerhitParticles[idx].Play();
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
                m_HomerunText.gameObject.SetActive(false);
            }

            m_HomerunhitParticle.Play();

            // Explode
            Vibrate(3.0f);
            var player = GameObject.FindGameObjectWithTag("Player").transform.position;
            Vector3 dir = (player - pos).normalized;
            m_HomerunText.position = pos + dir * 10f;
            m_HomerunText.rotation = Quaternion.LookRotation(dir,Vector3.up);
            m_HomerunText.gameObject.SetActive(true);
            foreach (Rigidbody rb in m_HomerunText.GetComponentsInChildren<Rigidbody>()) {
                rb.velocity = Vector3.zero;
                rb.transform.localPosition = Vector3.zero;
                rb.transform.localRotation = Quaternion.identity;
                rb.AddExplosionForce(50.0f, pos, 100.0f, 50.0f, ForceMode.Impulse);
                StartCoroutine(Explode(rb.transform, Vector3.zero, Vector3.one, 3.0f));
            }
        }


        IEnumerator Explode(Transform transform, Vector3 start, Vector3 end, float time)
        {
            float remain = time;
            while (remain > 0.0f)
            {
                remain -= Time.deltaTime;
                float t = 1 - remain / time;
                t = EaseOutElastic(t);
                transform.localScale = Vector3.Lerp(start, end, t);
                yield return null;
            }
            //yield return new WaitForSeconds(5);
        }
        public static float EaseOutElastic(float value)
        {
            float start = 0.0f;
            float end = 1.0f;
            end -= start;

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

        public void Vibrate(float duration)
        {
            OVRHapticsClip clip = new OVRHapticsClip();
            for (int i = 0; i < duration * 320; i++)
            {
                clip.WriteSample((byte)Mathf.Clamp((int)(0.4 * 255), 0, 255));
            }
            OVRHaptics.RightChannel.Preempt(clip);
            OVRHaptics.LeftChannel.Preempt(clip);
        }

    }
}