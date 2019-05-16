using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowards : MonoBehaviour
{
    public Vector3 targetPosition;
    public float moveSpeed = 1;
    public float stopDistance = 3;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, targetPosition) > stopDistance)
        {
            Vector3 moveDir = targetPosition - transform.position;
            transform.position += moveDir.normalized * moveSpeed * Time.fixedDeltaTime;
        }
    }
}
