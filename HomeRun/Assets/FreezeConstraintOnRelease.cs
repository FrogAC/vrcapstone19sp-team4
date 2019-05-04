using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeConstraintOnRelease : MonoBehaviour
{
    public OVRGrabbable grabbable;
    public Rigidbody rb;
    // Start is called before the first frame update

    void Start()
    {
        grabbable = GetComponent<OVRGrabbable>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!grabbable.isGrabbed)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
        else
        {
            rb.constraints = RigidbodyConstraints.None;
        }
    }
}
