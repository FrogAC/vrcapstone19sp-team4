using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * How to use this script:
 * 1. Attach this to a gameobject in the scene
 * 2. Enter in how much time should elapse before the scene change
 * 3. Enter the scene index of the scene you want to switch to
 * 4. Call the StartTimerFunction on a game event to start the timer
 * 
 * If you want the timer to start right away, call StartTimer in the start funtion
 * */

public class SceneChangeTimer : MonoBehaviour {

    [SerializeField]
    float time;

    [SerializeField]
    int sceneIndex;
    [SerializeField]
    string sceneName;

    bool didTimerStart = false;

    [SerializeField]
    bool startTimerOnStart;

    public void StartTimer()
    {
        didTimerStart = true;
    }

	// Use this for initialization
	void Start () {
        if (startTimerOnStart)
        {
            StartTimer();
        }
	}
	
	// Update is called once per frame
	void Update () {

        //this should be in a coroutine that gets started when appropriate, in update() it is checking the bool didTimerStart every frame which is costing performance - Rob
		if (didTimerStart)
        {
            time = time - Time.deltaTime;
            if (time <= 0.0f)
            {
                //defaults to using the written scene name to load the next scene, if blank then it loads from the scene number. -Rob
                if (sceneName == "")
                {
                    SceneManager.LoadSceneAsync(sceneIndex);
                } else
                {
                    SceneManager.LoadSceneAsync(sceneName);
                }
            }
        }
	}
    

}
