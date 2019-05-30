using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HomeRun.Net;
using TMPro;

public class MatchStartTrigger : MonoBehaviour
{
    [SerializeField] private MatchController matchController;
    [SerializeField] private PlayerType playerType = PlayerType.Batter;
    private static int s_hitRequired = 2;
    private static int s_hitLeft;
    private static int s_lastTargetID = 0;
    private string[] hints = new string[2];  // leave 2 for now

    private TextMeshPro tm;

    void Awake()
    {
        if (!matchController)
        {
            matchController = GameObject.FindObjectOfType<MatchController>();
        }

        tm = this.GetComponent<TextMeshPro>();
        string mType = (playerType == PlayerType.Batter ? "Batter" : "Pitcher");
        hints[1] = "Hit ME to join as " + mType + " !";
        hints[0] = "Hit AGAIN to confirm as" + mType + "!";

        SetHint(s_hitRequired);
    }

    public void SetHint(int left)
    {
        // left > 0
        tm.text = hints[left - 1];
    }

    void OnTriggerEnter(Collider other)
    {
        if (this.GetInstanceID() != s_lastTargetID)
        {  // reset id
            s_lastTargetID = this.GetInstanceID();
            s_hitLeft = s_hitRequired;
            foreach (var trigger in GameObject.FindObjectsOfType<MatchStartTrigger>())
                if (trigger != this) trigger.SetHint(s_hitRequired);
        }
        s_hitLeft--;

        if (s_hitLeft == 0)
        {
            MatchController.PlayerType = playerType;
            matchController.PlayOnlineOrCancel();
        }
        else
        {
            SetHint(s_hitLeft);
        }
    }
}
