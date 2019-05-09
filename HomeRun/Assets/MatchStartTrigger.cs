using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HomeRun.Net;

public class MatchStartTrigger : MonoBehaviour
{
    [SerializeField] private MatchController matchController;
    void OnTriggerEnter(Collider other) {
        // Based on Type?
        matchController.PlayOnlineOrCancel();
    }
}
