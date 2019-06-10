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
    private bool m_isDuringHit = false;
    [SerializeField] private Transform m_rotationTransform;

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
        if (m_isDuringHit) return;
        StartCoroutine(RotateOutBack(m_rotationTransform, 2.0f));
        
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

    IEnumerator RotateOutBack(Transform transform, float time)
    {
        m_isDuringHit = true;
        float remain = time;
        float start = 0.0f;
        float end = 360.0f;
        while (remain > 0.0f)
        {
            remain -= Time.deltaTime;
            float t = 1 - remain / time;
            t = EaseOutElasticBack(start, end, t);
            transform.rotation = Quaternion.AngleAxis(t, Vector3.up);
            yield return null;
        }
        m_isDuringHit = false;
    }

    public static float EaseOutElasticBack(float start, float end, float value)
    {
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
