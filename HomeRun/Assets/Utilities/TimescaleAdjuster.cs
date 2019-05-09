using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimescaleAdjuster : MonoBehaviour
{
    public KeyCode resetTimescale = KeyCode.J;
    public KeyCode slowMotion = KeyCode.K;
    public float slowMotionSpeed = 0.3f;
    public KeyCode pause = KeyCode.L;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(resetTimescale))
        {
            Time.timeScale = 1;
        } else if (Input.GetKeyDown(slowMotion))
        {
            Time.timeScale = slowMotionSpeed;
        } else if (Input.GetKeyDown(pause))
        {
            Time.timeScale = 0;
        }
    }
}
