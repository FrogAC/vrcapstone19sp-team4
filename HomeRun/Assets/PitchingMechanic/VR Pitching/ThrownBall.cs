using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;
using UnityEngine.Events;
using HomeRun.Net;
using HomeRun;
using HomeRun.Game;

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
    [SerializeField] AnimationCurve redirectionStrength = AnimationCurve.Linear(0, 0, 1, 1);
    [Space]
    // How strongly is the Spiral motion applieds
    [SerializeField] AnimationCurve SpiralAnimStrength = AnimationCurve.Linear(0, 1, 1, 0);
    [SerializeField] float spiralSpeed = 1f;
    [Space]
    public float vibrationFrequency = 0.4f;
    public float vibrationAmplitude = 0.4f;
    public float vibrationDuration = 0.1f;
    [Space]

    Vector3 releaseLinVel;
    Vector3 releaseAngVel;

    Vector3 flightVel;
    private OVRGrabber prevGrabber;

    private Collider m_collider;

    new private void Start()
    {
        base.Start();
        initialize();
    }

    // ugly interface
    public BallType balltype;

    override public void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        if (GlobalSettings.UseNetwork && PlatformManager.CurrentState == PlatformManager.State.PLAYING_A_NETWORKED_MATCH
            && MatchController.PlayerType == PlayerType.Pitcher)
                NetStrikeZone.strikezone.SetMotion(false);

        prevGrabber = grabbedBy;

        base.GrabEnd(linearVelocity, angularVelocity);

        releaseLinVel = linearVelocity;
        releaseAngVel = angularVelocity;
        //Debug.Log("GrabEnd Success!");
        if (GlobalSettings.UseNetwork && MatchController.PlayerType == PlayerType.Pitcher)
        {
            PlatformManager.Instance.P2PThrowBall(gameObject.GetInstanceID(), transform.position,
                                                releaseLinVel, NetStrikeZone.strikezone.transform.position);
        }

        StartCoroutine(Throw());
    }

    public void initialize()
    {
        m_collider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        strikezone = Strikezone.strikezone.transform;
        strikezoneCollider = strikezone.GetComponent<Collider>();
    }
    
    public void StopThrow() {
        StopAllCoroutines();
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
        rb.useGravity = true;
        // play effect and destroy ball on success strike
        if (GlobalSettings.UseNetwork) {
            NetEffectController.Instance.PlayStrikeZoneHitEffect(transform.position, BallType.FastBall);
            Destroy(gameObject);
        }
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
        if (strikezoneCollider != null)
        {
            return strikezoneCollider.ClosestPoint(transform.position);
        }

        return strikezone.position;
    }

    private bool hasHitBat = false;
    public float batHitMult = 3.0f;
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
            if (collision.gameObject.layer == LayerMask.NameToLayer("Bat") && !hasHitBat)
            {
                // san check
                Physics.IgnoreCollision(collision.collider, m_collider, true);
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                hasHitBat = true;


                //onHitByBat.Invoke();

                // Launch ball
                Debug.DrawRay(transform.position, collision.GetContact(0).normal, Color.black, 1.0f);

                Vector3 nVel = Vector3.Reflect(-collision.relativeVelocity, collision.GetContact(0).normal);
                Vector3 LaunchDir = Vector3.ProjectOnPlane(nVel, collision.collider.transform.up);

                nVel = LaunchDir.normalized * nVel.magnitude * batHitMult;

                float pointSpeed = 1;
                Rigidbody batRB = collision.rigidbody;
                if (batRB != null)
                {
                    //Debug.Log(batRB.velocity + "------" + batRB.angularVelocity);
                    pointSpeed = batRB.GetPointVelocity(transform.position).magnitude;
                    nVel *= (pointSpeed + 0.5f);
                }
                Debug.DrawRay(transform.position, nVel, Color.blue, 5.0f);

                rb.velocity = nVel;

               // Debug.Log("out" + nVel);
                // After all calculation
                if (GlobalSettings.UseNetwork) NetEffectController.Instance.PlayBatHitEffect(transform.position, balltype);
                OnHitByBat(transform.position, nVel);

                //Debug.DrawLine(collision.GetContact(0).point, collision.GetContact(0).point + rb.velocity.normalized * 2, Color.red, 3);

                //Debug.Log("pointSpeed = " + pointSpeed);

                //StartCoroutine(VibrateController(vibrationFrequency, vibrationAmplitude, vibrationDuration));
                OVRHapticsClip clip = new OVRHapticsClip();
                for (int i = 0; i < vibrationDuration * 320; i++)
                {
                    clip.WriteSample((byte)Mathf.Clamp((int)(vibrationAmplitude * 255 * (1 + pointSpeed / 10)), 0, 255));
                }
                OVRHaptics.RightChannel.Preempt(clip);
                OVRHaptics.LeftChannel.Preempt(clip);
            }
        }
    }

    // For NET
    public void OnHitByBat(Vector3 pos, Vector3 vel)
    {
        if (GlobalSettings.UseNetwork  && MatchController.PlayerType == PlayerType.Batter)
       {
            P2PNetworkBall netball = gameObject.GetComponent<P2PNetworkBall>();
            if (!netball)
            {
                Debug.Log("No NetWorkBall Found!");
                return;
            }
            Debug.Log("Sending Ball Hit:" + vel);
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
