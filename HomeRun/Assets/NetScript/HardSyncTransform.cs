using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardSyncTransform : MonoBehaviour
{
    [SerializeField] private Transform m_syncTransform;
    
    private Rigidbody rb;

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 swingDir = Vector3.Cross(rb.velocity.normalized, transform.forward).normalized;
        Debug.DrawRay(transform.position, swingDir, Color.red, .5f);
        //transform.position = m_syncTransform.position;
        //transform.rotation = m_syncTransform.rotation;
    }
}
