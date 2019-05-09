using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HomeRun.Net;

public class MatchStartTrigger : MonoBehaviour
{
    [SerializeField] private MatchController matchController;
    [SerializeField] private PlayerType playerType = PlayerType.Batter;

    void Awake() {
        if (!matchController) {
            matchController = GameObject.FindObjectOfType<MatchController>();
        }
    }

    void OnTriggerEnter(Collider other) {
        matchController.PlayerType = playerType;
        matchController.PlayOnlineOrCancel();

        var triggers = GameObject.FindObjectsOfType<MatchStartTrigger>();
        foreach (var trigger in triggers) {
            trigger.enabled = false;
        }
    }
}
