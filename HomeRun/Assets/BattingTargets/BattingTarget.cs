using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BattingTarget : MonoBehaviour
{
    public ParticleSystem destroyPS;
    public UnityEvent OnHitBall;
    public bool lookAtOnStart = true;

    private void Start()
    {
        if (lookAtOnStart)
        {
            transform.LookAt(Vector3.zero);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnTriggerEnter(collision.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ball"))
        {
            if (destroyPS != null)
            {
                ParticleSystem ps = Instantiate(destroyPS, transform.position, transform.rotation);
                ps.Play();
            }

            OnHitBall.Invoke();
            Destroy(gameObject);
        }
    }
}
