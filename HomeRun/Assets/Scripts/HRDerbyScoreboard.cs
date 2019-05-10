using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HRDerbyScoreboard : MonoBehaviour
{
    public Text timerText, scoreText;

    void Start()
    {
        UpdateScoreText(0);
        UpdateTimeText(60);
    }

    public void UpdateTimeText(int time)
    {
        int sec = time % 60;
        int min = time / 60;
        string res = (min >= 10) ? "" + min : ("0" + min);
        res += ":" + ((sec >= 10) ? "" + sec : ("0" + sec));
        timerText.text = res;
    }

    public void UpdateScoreText(int score)
    {
        string res = "";
        if (score < 10)
        {
            res += "0";
        }
        res += score;
        scoreText.text = res;
    }
}
