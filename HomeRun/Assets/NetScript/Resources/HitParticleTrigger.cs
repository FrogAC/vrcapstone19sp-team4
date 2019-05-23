using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HomeRun.Net;

public class HitParticleTrigger : MonoBehaviour
{

    void OnCollisionEnter(Collision other) {
        NetEffectController.Instance.PlayBatHitEffect(other.transform.position);
    }
}
