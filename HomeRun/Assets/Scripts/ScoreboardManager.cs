using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardManager : MonoBehaviour
{
    private Canvas scoreboard;
    private Text awayText, homeText, inningText;
    private int homeScore, awayScore, inning, balls, strikes, outs;

    // Start is called before the first frame update
    void Start()
    {
    
    }

    public void IncrementInning()
    {
        SetInning(inning + 1);
    }

    public void SetInning(int inning)
    {
        this.inning = inning;
        inningText.text = this.inning.ToString();
    }

    public void IncrementHomeScore()
    {
        homeScore++;
        UpdateHomeScore();
    }

    public void SetAwayNull()
    {
        awayText.text = "--";
    }

    public void IncrementAwayScore()
    {
        awayScore++;
        UpdateAwayScore();
    }

    public void UpdateAwayScore()
    {
        if (awayScore < 10)
        {
            awayText.text = "0";
        }
        awayText.text += awayScore.ToString();
    }

    public void UpdateHomeScore()
    {
        if (homeScore < 10)
        {
            homeText.text = "0";
        }
        homeText.text += homeScore.ToString();
    }

    public int GetInning() { return inning; }
    public int GetHomeScore() { return homeScore; }
    public int GetAwayScore() { return awayScore; }
    public int GetBalls() { return balls; }
    public int GetStrikes() { return strikes; }
    public int GetOuts() { return strikes; }
}
