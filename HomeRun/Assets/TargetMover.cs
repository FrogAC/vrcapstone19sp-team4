using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMover : MonoBehaviour
{
    public bool loopPath = true;
    public Vector3[] waypoints;
    public int currentIndex = 0;

    public float moveSpeed = 5;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        // Move along paths defined by waypoints
        if (currentIndex < waypoints.Length) {
            MoveTowardsCurrentWaypoint(Time.deltaTime);
            if (transform.position == waypoints[currentIndex]) {
                currentIndex++;
                // Loops back to start when passing last waypoint
                if (loopPath && currentIndex >= waypoints.Length) {
                    currentIndex = 0;
                }
            }
        }
    }

    void MoveTowardsCurrentWaypoint(float timestep){
        // Determine how far to move, without moving past target
        float moveDistance = moveSpeed * timestep;
        moveDistance = Mathf.Min(Vector3.Distance(transform.position, waypoints[currentIndex]), moveDistance);

        Vector3 targetVector = waypoints[currentIndex] - transform.position;
        
        transform.position += targetVector.normalized * moveDistance;
    }

    void OnDrawGizmos(){
        Gizmos.color = Color.red;
        for(int i = 0; i < waypoints.Length-1; i++){
            Gizmos.DrawLine(waypoints[i], waypoints[i+1]);
        }
        if (loopPath){
            Gizmos.DrawLine(waypoints[waypoints.Length - 1], waypoints[0]);
        }
    }
}
