using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleOverTime : MonoBehaviour
{
    public float unitsPerSecond = 1;

    [Header("Bounds for scale change relative to starting scale")]
    public float lowerBound = 0.1f;
    public float upperBound = 10;

    private Vector3 startingScale;
    void Start (){
        startingScale = transform.localScale;
    }
    void Update()
    {
        float relativeScale = transform.localScale.magnitude/startingScale.magnitude;

        // Enforces bounds
        if ((unitsPerSecond > 0 && relativeScale > upperBound) || (unitsPerSecond < 0 && relativeScale < lowerBound)) {
            return;
        }
        
        transform.localScale += transform.localScale.normalized * unitsPerSecond * Time.deltaTime;
    }
}
