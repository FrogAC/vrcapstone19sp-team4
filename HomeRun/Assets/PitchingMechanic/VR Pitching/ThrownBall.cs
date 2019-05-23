using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;
using UnityEngine.Events;
using HomeRun.Net;
using HomeRun;

public class ThrownBall : OVRGrabbable
{
    [Space]
    public float speed = 3;

    Vector3 releasePosition;
    [SerializeField] public Transform strikezone;
    private Collider strikezoneCollider;
    private Rigidbody rb;
    [Header("Path properties")]
    // How strongly does the ball curve to the strike zone at different parts of the throw
    [SerializeField] AnimationCurve redirectionStrength = AnimationCurve.Linear(0,0,1,1);
    [Space]
    // How strongly is the Spiral motion applieds
    [SerializeField] AnimationCurve SpiralAnimStrength = AnimationCurve.Linear(0, 1, 1, 0);
    [SerializeField] float spiralSpeed = 1f;
    [Space]
    public float vibrationFrequency = 0.4f;
    public float vibrationAmplitude = 0.4f;
    public float vibrationDuration = 0.1f;
    [Space]
    [SerializeField] UnityEvent onHitByBat;

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
        if (GlobalSettings.UseNetwork && PlatformManager.CurrentState == PlatformManager.State.PLAYING_A_NETWORKED_MATCH && MatchController.PlayerType == PlayerType.Pitcher) {
            PlatformManager.Instance.P2PThrowBall(gameObject.GetInstanceID(), transform.position, releaseLinVel);
        }

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

    private bool hasHitBat = false;
    public float batHitMult = 3;
    private void OnCollisionEnter(Collision collision)
    {
        // Remote Balls Bug fix, Not sure actual reason.
        if (!rb) return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Environment"))
        {
            Destroy(gameObject, 4);
        }

        // Ignore collision with hands
        if (collision.gameObject.layer != LayerMask.NameToLayer("PlayerBody"))
        {
            StopAllCoroutines();
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;
            //rb.velocity = Vector3.Reflect(flightVel.normalized, collision.impulse.normalized) * flightVel.magnitude;
            //Debug.Log(name + " hit " + collision.gameObject.name);
            
            
            // BatHit functionality
            if(collision.gameObject.layer == LayerMask.NameToLayer("Bat") && !hasHitBat)
            {
                hasHitBat = true;

                onHitByBat.Invoke();

                Physics.IgnoreCollision(collision.collider, collision.GetContact(0).otherCollider, true);
                // Launch ball
                rb.velocity = Vector3.Reflect(rb.velocity, collision.GetContact(0).normal);
                Vector3 LaunchDir = Vector3.ProjectOnPlane(rb.velocity, collision.collider.transform.up);

                rb.velocity = LaunchDir.normalized * rb.velocity.magnitude * batHitMult;

                float pointSpeed = 1;
                Rigidbody batRB = collision.gameObject.GetComponentInParent<OVRGrabbable>().GetComponent<Rigidbody>();
                if (batRB != null)
                {
                    //Debug.Log(batRB.velocity + "------" + batRB.angularVelocity);
                    pointSpeed = batRB.GetPointVelocity(collision.GetContact(0).point).magnitude;
                    rb.velocity *= pointSpeed;
                }

                // After all calculation
                OnHitByBat(transform.position, rb.velocity);

                Debug.DrawLine(collision.GetContact(0).point, collision.GetContact(0).point + rb.velocity.normalized * 2, Color.red, 3);

                Debug.Log("pointSpeed = " + pointSpeed);

                //StartCoroutine(VibrateController(vibrationFrequency, vibrationAmplitude, vibrationDuration));
                OVRHapticsClip clip = new OVRHapticsClip();
                for (int i = 0; i < vibrationDuration * 320; i++)
                {
                    clip.WriteSample((byte)Mathf.Clamp((int)(vibrationAmplitude * 255 * (1 + pointSpeed/10)), 0, 255));
                }
                OVRHaptics.RightChannel.Preempt(clip);
                OVRHaptics.LeftChannel.Preempt(clip);

            }
        }
    }

    // For NET
    public void OnHitByBat(Vector3 pos, Vector3 vel) {
        if (GlobalSettings.UseNetwork && PlatformManager.CurrentState == PlatformManager.State.PLAYING_A_NETWORKED_MATCH && MatchController.PlayerType == PlayerType.Batter) {
            P2PNetworkBall netball = gameObject.GetComponent<P2PNetworkBall>();
            if (!netball) {
                Debug.Log("No NetWorkBall Found!");
                return;
            }
			Debug.Log("Ball Hit Velocity:" + vel);
            PlatformManager.Instance.P2PHitBall(netball.InstanceID, pos, vel);
        }
    }


    /*
    public static int vibrationCounter = 0;
    IEnumerator VibrateController(float freqency, float amplitude, float duration)
    {


        /*
        vibrationCounter++;
        int curr = vibrationCounter;
        OVRInput.SetControllerVibration(freqency, amplitude, OVRInput.Controller.RTouch);
        OVRInput.SetControllerVibration(freqency, amplitude, OVRInput.Controller.LTouch);

        Debug.Log("Vibrate " + curr + " Start Time = " + Time.time);
        yield return new WaitForSeconds(duration);
        
        Debug.Log("Vibrate " + curr + " End Time = " + Time.time);

        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);*/
    //}
}
