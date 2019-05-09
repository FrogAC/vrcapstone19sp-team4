using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HRDerbyScoreboardManager : MonoBehaviour
{
    public delegate void OnTimeUp();
    public event OnTimeUp TimeUpEvent;

    public Text timerText, scoreText;
    public int timeAllowed;
    private bool isPaused;
    private Coroutine timer;

    // Start is called before the first frame update
    void Start()
    {
        timeAllowed = 60;
        timer = StartCoroutine("Timer", timeAllowed);
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

    /// <summary>
    /// Starts Timer
    /// </summary>
    public void StartTimer()
    {
        //if (timer.)
        //{
            ResetTimer();
            timer = StartCoroutine(Timer(timeAllowed));
        //}
    }

    /// <summary>
    /// Resets Timer to original allowed time
    /// </summary>
    public void ResetTimer()
    {
        StopCoroutine(timer);
        isPaused = false;
        UpdateTimeText(timeAllowed);
    }

    /// <summary>
    /// Pauses Timer
    /// </summary>
    public void PauseTimer()
    {
        isPaused = true;
    }

    /// <summary>
    /// Resumes timer
    /// </summary>
    public void ResumeTimer()
    {
        if (isPaused) 
            isPaused = false;
    }

    /// <summary>
    /// Sets correct time string on GUI text
    /// </summary>
    /// <param name="time">
    /// int time remaining in seconds
    /// </param>
    private void UpdateTimeText(int time)
    {
        int sec = time % 60;
        int min = time / 60;
        string res = (min >= 10) ? "" + min : ("0" + min);
        res += ":" + ((sec >= 10) ? "" + sec : ("0" + sec));
        timerText.text = res;
    }

    private void IncrementHRCount()
    {

    }
}
