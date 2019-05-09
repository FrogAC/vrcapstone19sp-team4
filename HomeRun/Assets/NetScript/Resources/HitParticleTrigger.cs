using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitParticleTrigger : MonoBehaviour
{
    [SerializeField] private ParticleSystem prefab;

    private ParticleSystem ps;

    void Awake() {
        ps = Instantiate(prefab, transform.position, transform.rotation);
        ps.transform.SetParent(transform);
    }
    void OnCollisionEnter(Collision other) {
        ps.transform.position = other.transform.position;
        ps.Play();
    }
}
