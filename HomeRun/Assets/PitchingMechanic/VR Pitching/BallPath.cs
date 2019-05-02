using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BallPath/simple")]
public class BallPath : ScriptableObject
{
    public float strengthMultiplier = 1;
    public AnimationCurve animationStrength;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 getAugmentation(float normalizedProgress)
    {
        return Vector3.forward;
    }
}
