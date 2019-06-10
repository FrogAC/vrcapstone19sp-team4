using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicRotatingCamera : MonoBehaviour
{
    
    public Vector2 rotationSpeed;

    void Start()
    {
        StartCoroutine(RotateOverTime());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator RotateOverTime(){
        while (true) {
            transform.RotateAround(transform.position, Vector3.down, rotationSpeed.x * Time.deltaTime);
            transform.RotateAround(transform.position, transform.right, rotationSpeed.y * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
}
