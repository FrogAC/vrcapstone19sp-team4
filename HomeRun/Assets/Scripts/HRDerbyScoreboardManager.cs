using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HRDerbyScoreboardManager : MonoBehaviour
{
    public delegate void OnTimeUp();
    public event OnTimeUp TimeUpEvent;

    public Text timerText, scoreText;
    public int timeAllowed, score;
    private bool isPaused;
    private Coroutine timer;

    // Start is called before the first frame update
    void Start()
    {
        timeAllowed = 60;
        //timer = StartCoroutine("Timer", timeAllowed);
        isPaused = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PauseTimer();   
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            ResumeTimer();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetTimer();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            StartTimer();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            IncrementHRCount();
        }
    }

    // Coroutine Runs Timer
    IEnumerator Timer(int time)
    {
        int i = time;
        while (i >= 0)
        {
            while (isPaused)
            {
                yield return null;
            }
            UpdateTimeText(i);
            if (i == 0)
            {
                // TimeUpEven
                if (TimeUpEvent != null)
                {
                    TimeUpEvent();
                }
            }
            yield return new WaitForSeconds(1);
            i -= 1;
        }
    }

    public void StartTimer()
    {
        ResetTimer();
        timer = StartCoroutine(Timer(timeAllowed));
    }

    public void ResetTimer()
    {
        if (timer != null)
            StopCoroutine(timer);

        isPaused = false;
        UpdateTimeText(timeAllowed);
    }

    public void PauseTimer()
    {
        isPaused = true;
    }

    public void ResumeTimer()
    {
        if (isPaused) 
            isPaused = false;
    }

    private void UpdateTimeText(int time)
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

    private void IncrementHRCount()
    {
        score += 1;
        UpdateScoreText(score);
    }
}
