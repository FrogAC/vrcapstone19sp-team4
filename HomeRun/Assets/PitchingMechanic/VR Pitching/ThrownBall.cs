using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;

public class ThrownBall : OVRGrabbable
{
    [Space(10)]
    public float speed = 3;

    Vector3 releasePosition;
    [SerializeField] public Transform strikezone;
    private Collider strikezoneCollider;
    private Rigidbody rb;
    [Header("Path properties")]
    // How strongly does the ball curve to the strike zone at different parts of the throw
    [SerializeField] AnimationCurve redirectionStrength = AnimationCurve.Linear(0,0,1,1);
    [Space(10)]
    // How strongly is the Spiral motion applieds
    [SerializeField] AnimationCurve SpiralAnimStrength = AnimationCurve.Linear(0, 1, 1, 0);
    [SerializeField] float spiralSpeed = 1f;

    Vector3 releaseLinVel;
    Vector3 releaseAngVel;

    Vector3 flightVel;
    private OVRGrabber prevGrabber;

    new private void Start()
    {
        base.Start();
        initialize();
    }

    override public void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        prevGrabber = grabbedBy;

        base.GrabEnd(linearVelocity, angularVelocity);

        releaseLinVel = linearVelocity;
        releaseAngVel = angularVelocity;
        //Debug.Log("GrabEnd Success!");
        GetComponent<Renderer>().material.color = Color.red;

        StartCoroutine(Throw());
    }

    public void initialize()
    {
        rb = GetComponent<Rigidbody>();
        strikezone = Strikezone.strikezone.transform;
        strikezoneCollider = strikezone.GetComponent<Collider>();
    }

    IEnumerator Throw()
    {
        
        rb.isKinematic = false;
        //Debug.Log("throw()");
        //transform.LookAt(strikezone);
        Vector3 releasePos = transform.position;
        

        transform.forward = releaseLinVel.normalized;

        // Do FixedUpdate loop until ball is past the strikezone
        float dist = 0;
        while (dist < 1)
        {
            dist = GetNormalizedPitchProgress(releasePos, transform.position);

            Vector3 pitchingLine = releasePos - GetBallTargetPosition();
            Vector3 targetVector = GetBallTargetPosition() - transform.position;
            //Debug.Log(dist+ "\t\t"+targetVector);

            //Applies Spiral Motion
            //Vector3 trajectoryAugmentation = ballTarget.right * Mathf.Sin(dist*spiralspeed) + ballTarget.up * Mathf.Cos(dist * spiralspeed);
            Vector3 normal = -Vector3.ProjectOnPlane(targetVector, pitchingLine);
            Vector3 bitangent = Vector3.Cross(pitchingLine, normal);

            bitangent = bitangent.normalized + -normal.normalized * spiralSpeed;

            bitangent.Normalize();

            transform.forward = Vector3.Lerp(transform.forward, bitangent.normalized, SpiralAnimStrength.Evaluate(dist)).normalized;
            // Calculates redirection
            transform.forward = Vector3.Lerp(transform.forward, targetVector.normalized, redirectionStrength.Evaluate(dist)).normalized + transform.forward * Mathf.Epsilon;
            // Moves the ball in the direction it's facing
            //transform.position += transform.forward * Time.fixedDeltaTime * speed * releaseLinVel.magnitude;
            flightVel = transform.forward * speed * releaseLinVel.magnitude;

            rb.angularVelocity = Vector3.zero;
            rb.velocity = flightVel;

            yield return new WaitForFixedUpdate();
        }
        //Debug.Log("throw() done");
    }

    float GetNormalizedPitchProgress(Vector3 releasePos, Vector3 position)
    {
        // Vector from pitcher to the strikezone
        Vector3 pitchingLine = releasePos - GetBallTargetPosition();
        // Vector from position to strikezone
        Vector3 targetVector = position - GetBallTargetPosition();

        Vector3 vectorAlongPitchingLine = Vector3.Project(targetVector, pitchingLine);
        float pitchProgress = (pitchingLine - vectorAlongPitchingLine).magnitude;

        return pitchProgress / pitchingLine.magnitude;
    }

    Vector3 GetBallTargetPosition()
    {
        if(strikezoneCollider != null)
        {
            return strikezoneCollider.ClosestPoint(transform.position);
        }

        return strikezone.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Ignore collision with hands
        if (collision.gameObject.layer != LayerMask.NameToLayer("PlayerBody"))
        {
            StopAllCoroutines();
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;
            //rb.velocity = Vector3.Reflect(flightVel.normalized, collision.impulse.normalized) * flightVel.magnitude;
            //Debug.Log(name + " hit " + collision.gameObject.name);
        }
    }
    /*
    private void OnTriggerEnter(Collider other)
    {
        StopCoroutine(Throw());
        Debug.Log(name + " triggered " + other.gameObject.name);
    }
    */


}
