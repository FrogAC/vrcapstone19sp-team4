using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class TargetPracticeManager : MonoBehaviour
{
    // Parent of all targets that should be tracked
    public GameObject targetsContainer;
    public int remainingTargets = 1;
    [Space]
    public TextMeshProUGUI targetCountText;
    public TextMeshProUGUI timerText;

    public UnityEvent OnGameCompleteEvent;
    private bool hasCompleted = false;

    private float completionTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        remainingTargets = targetsContainer.transform.childCount;
        if (remainingTargets > 0)
        {
            targetCountText.text = "Targets: \n" + remainingTargets;
            timerText.text = "Time: \n"+ string.Format("{0:00}:{1:00.00}", (int)Time.timeSinceLevelLoad / 60, Time.timeSinceLevelLoad % 60);
        } else
        {
            if (! hasCompleted) {
                hasCompleted = true;
                OnGameCompleteEvent.Invoke();
            }

            if (completionTime <= 0)
            {
                completionTime = Time.timeSinceLevelLoad;
            }
            targetCountText.text = "All Targets Hit!";
            timerText.text = "Time: \n" + string.Format("{0:00}:{1:00.00}", (int)completionTime / 60, completionTime % 60);
        }
    }
}
