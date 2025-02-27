﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HRGameManager : MonoBehaviour
{
    public delegate void HomeRunDelegate();
    public event HomeRunDelegate homeRunEvent;
    public delegate void OnTimeUp();
    public event OnTimeUp TimeUpEvent;

    public HRDerbyScoreboard scoreboard;
    public OVRGrabbable bat;
    public int timeAllowed;

    private int homeruns;
    private bool isPaused, batGrabbed;
    private Coroutine timer;
    

    private void Start()
    {
        // Init Variables
        batGrabbed = false;
        if (timeAllowed <= 0)
            timeAllowed = 60;
        isPaused = false;
        TimeUpEvent += GameOver;
    }

    private void Update()
    {
        if (bat.isGrabbed && !batGrabbed)
        {
            StartTimer();
            batGrabbed = true;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PauseGame();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            ResumeGame();
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
            HomeRun();
        }
    }

    //====================================================
    // Scoring Methods
    //====================================================

    public void HomeRun()
    {
        homeruns++;
        scoreboard.UpdateScoreText(homeruns);
        if (homeRunEvent != null)
        {
            homeRunEvent();
        }
    }

    public void GameOver()
    {
        TimeUpEvent -= GameOver;
        Debug.Log("GameOver");
    }

    

    //====================================================
    // Game Flow Methods
    //====================================================

    public void StartGame()
    {

    }

    public void PauseGame()
    {
        if (!isPaused)
            isPaused = true;
    }

    public void ResumeGame()
    {
        if (isPaused)
            isPaused = false;
    }

    public void ResetGame()
    {
        
    }

    //====================================================
    // Timer Methods
    //====================================================

    public void StartTimer()
    {
        ResetTimer();
        timer = StartCoroutine(Timer(timeAllowed));
        TimeUpEvent += GameOver;
    }

    public void ResetTimer()
    {
        if (timer != null)
            StopCoroutine(timer);

        isPaused = false;
        scoreboard.UpdateTimeText(timeAllowed);
        
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
            scoreboard.UpdateTimeText(i);
            if (i == 0)
            {
                // TimeUpEven
                if (TimeUpEvent != null)
                {
                    TimeUpEvent();
                }
            }
            i -= 1;
            yield return new WaitForSeconds(1);
        }
    }

    //====================================================
    // Accessors
    //====================================================
}
