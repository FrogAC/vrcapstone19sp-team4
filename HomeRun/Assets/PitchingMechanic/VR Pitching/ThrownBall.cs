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
    // Uses values from 0 to 1. How much should the bat's launch direction be influenced by curved surfaces
    //   0 = only use contact normals, 1 = only use swing direction
    public static float BATTING_AIM_ASSIST = 0.5f;
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

    [SerializeField] AnimationCurve JitterAnimStrength = AnimationCurve.Linear(0, 0, 1, 0);
    [SerializeField] float jitterSpeed = 1f;
    [SerializeField] float jitterHeightOffset = 0;
    [Space]
    
    [SerializeField] AnimationCurve controlStrength = AnimationCurve.Linear(0, 0, 1, 0);
    [Space]

    // Normalized speed relative to launch velocity of the ball
    [SerializeField] AnimationCurve NormalizedSpeedOverPath = AnimationCurve.Linear(0, 1, 1, 1);
    [SerializeField] bool normailzedSpeedAffectsUpdateDelay;
    [Space]
    [SerializeField] float updateDelay = 0;
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
    [SerializeField] private GameObject m_effectObj;
    [SerializeField] private float m_throwSpeedThreshold = 1.0f;
    private bool m_enableFX = false;
    private bool m_hasStrike = false;
    private bool m_hasHitEnv = false;
    public void SetEnableFX(bool value)
    {
        m_enableFX = value;
        if (m_effectObj)
            m_effectObj.SetActive(value);
    }

    void Awake()
    {
        initialize();
    }

    // ugly interface
    public BallType balltype;

    override public void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        SetEnableFX(true);
        if (GlobalSettings.UseNetwork)
            NetStrikeZone.strikezone.SetMotion(false);

        prevGrabber = grabbedBy;

        base.GrabEnd(linearVelocity, angularVelocity);

        releaseLinVel = linearVelocity;
        releaseAngVel = angularVelocity;
        //Debug.Log("GrabEnd Success!");
        if (GlobalSettings.UseNetwork && MatchController.PlayerType == PlayerType.Pitcher)
        {
            PlatformManager.Instance.P2PThrowBall(gameObject.GetInstanceID(), transform.position,
                                                releaseLinVel, NetStrikeZone.strikezone.transform.position);  // an actually badd idea
                                                // but works
        }


        // check releasethreadhold, use RAW speed!
        if (releaseLinVel.magnitude < m_throwSpeedThreshold)
        {
            SetEnableFX(true);
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.velocity = releaseLinVel * 5;
        }
        else
        {
            StartCoroutine(Throw());
        }
    }

    void initialize()
    {
        SetEnableFX(false);
        m_collider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        strikezone = Strikezone.strikezone.transform;
        strikezoneCollider = strikezone.GetComponent<Collider>();
    }

    public void StopThrow()
    {
        StopAllCoroutines();
    }

    IEnumerator Throw()
    {
        rb.isKinematic = false;
        Vector3 releasePos = transform.position;

        transform.forward = releaseLinVel.normalized;

        // Do FixedUpdate loop until ball is past the strikezone
        float dist = 0;
        while (dist < 1)
        {
            dist = GetNormalizedPitchProgress(releasePos, transform.position);

            // Reference directions used in calculations
            Vector3 pitchingLine = releasePos - GetBallTargetPosition();
            Vector3 targetVector = GetBallTargetPosition() - transform.position;

            // Applies Control
            if (prevGrabber != null) {
                // Gets direction that the controller is pointing in order to allow direct control of trajectory
                Vector3 controlVec = Vector3.ProjectOnPlane(prevGrabber.transform.forward, pitchingLine);
                controlVec *= controlStrength.Evaluate(dist);
                transform.forward = Vector3.Lerp(transform.forward, controlVec, controlStrength.Evaluate(dist)).normalized + transform.forward * Mathf.Epsilon;
            }

            //Applies Spiral Motion
            //Vector3 trajectoryAugmentation = ballTarget.right * Mathf.Sin(dist*spiralspeed) + ballTarget.up * Mathf.Cos(dist * spiralspeed);
            Vector3 normal = -Vector3.ProjectOnPlane(targetVector, pitchingLine);
            Vector3 bitangent = Vector3.Cross(pitchingLine, normal);
            bitangent = bitangent.normalized + -normal.normalized * spiralSpeed;
            bitangent.Normalize();
            transform.forward = Vector3.Lerp(transform.forward, bitangent.normalized, SpiralAnimStrength.Evaluate(dist)).normalized;

            // Applies Jitter
            Vector3 jitterVec = new Vector3(Mathf.PerlinNoise(0, Time.time * jitterSpeed), Mathf.PerlinNoise(Time.time * jitterSpeed, 0), 0);
            jitterVec -= new Vector3(0.5f, transform.position.y - releasePos.y - jitterHeightOffset, 0);   // Adjusts random direction to avoid hitting ground
            transform.forward = Vector3.Lerp(transform.forward, jitterVec.normalized, JitterAnimStrength.Evaluate(dist)).normalized;

            // Calculates redirection
            transform.forward = Vector3.Lerp(transform.forward, targetVector.normalized, redirectionStrength.Evaluate(dist)).normalized + transform.forward * Mathf.Epsilon;
            // Moves the ball in the direction it's facing
            //transform.position += transform.forward * Time.fixedDeltaTime * speed * releaseLinVel.magnitude;
            flightVel = transform.forward * speed * releaseLinVel.magnitude;

            rb.angularVelocity = Vector3.zero;
            rb.velocity = flightVel;

            // Applys the update delay, calculating the delay if dependent on speed
            float effectiveDelay = updateDelay;
            updateDelay /= (normailzedSpeedAffectsUpdateDelay) ? NormalizedSpeedOverPath.Evaluate(dist) : 1;
            yield return new WaitForSeconds(effectiveDelay);

            yield return new WaitForFixedUpdate();
        }
        rb.useGravity = true;
        GlobalSettings.Selectable = true;
        // play effect and destroy ball on success strike
        if (GlobalSettings.UseNetwork)
        {
            Debug.LogError("NetStrikeHit! Should either hit True StikeZone or Miss befire this!");
           // Destroy(gameObject);
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

        if (collision.gameObject.layer == LayerMask.NameToLayer("Environment") && !m_hasHitEnv)
        {
            m_hasHitEnv = true;
            Destroy(gameObject, 4);
            GlobalSettings.Selectable = true;
            if (GlobalSettings.UseNetwork) NetEffectController.Instance.PlayGroundHitEffect(transform.position);
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
                //Physics.IgnoreCollision(collision.collider, m_collider, true);
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                hasHitBat = true;


                //onHitByBat.Invoke();

                // Launch ball
                Debug.DrawRay(transform.position, collision.GetContact(0).normal, Color.black, 1.0f);

                Vector3 nVel = Vector3.Reflect(-collision.relativeVelocity, collision.GetContact(0).normal);
                Vector3 LaunchDir = Vector3.ProjectOnPlane(nVel, collision.collider.transform.up);

                nVel = LaunchDir.normalized * nVel.magnitude * batHitMult;
                Debug.DrawRay(transform.position, nVel, Color.blue, 5.0f);

                float pointSpeed = 1;
                Rigidbody batRB = collision.rigidbody;
                if (batRB != null)
                {
                    // Applies Bat Aim Assist - Calculates the direction of the swing
                    Vector3 swingDir = Vector3.Cross(batRB.angularVelocity.normalized, batRB.transform.up).normalized;
                    Debug.DrawRay(transform.position, swingDir, Color.red, 5.0f);
                    nVel = Vector3.Lerp(nVel.normalized, swingDir.normalized, BATTING_AIM_ASSIST).normalized * nVel.magnitude;

                    //Debug.Log(batRB.velocity + "------" + batRB.angularVelocity);
                    pointSpeed = batRB.GetPointVelocity(transform.position).magnitude;
                    nVel *= (pointSpeed + 0.5f);
                }

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

    void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.layer == LayerMask.NameToLayer("HomerunFence")) {
            if (GlobalSettings.UseNetwork &&  PlatformManager.CurrentState == PlatformManager.State.PLAYING_A_NETWORKED_MATCH) 
                NetEffectController.Instance.PlayHomerunEffect(transform.position);
            // Audio etc
        } else if (collider.tag.Equals("StrikeTrigger") && !hasHitBat) {
            m_hasStrike = true;
            StopAllCoroutines();
            rb.useGravity = true;
            GlobalSettings.Selectable = true;

            OVRHapticsClip clip = new OVRHapticsClip();
            for (int i = 0; i < vibrationDuration * 320; i++)
            {
                clip.WriteSample((byte)Mathf.Clamp((int)(vibrationAmplitude * 255 * (1 + releaseLinVel.magnitude / 10)), 0, 255));
            }
            OVRHaptics.RightChannel.Preempt(clip);
            OVRHaptics.LeftChannel.Preempt(clip);
            NetEffectController.Instance.PlayStrikeZoneHitEffect(transform.position);
        } else if (collider.tag.Equals("MissTrigger") && !m_hasStrike && !hasHitBat) {
            StopAllCoroutines();
            rb.useGravity = true;
            GlobalSettings.Selectable = true;

            OVRHapticsClip clip = new OVRHapticsClip();
            for (int i = 0; i < vibrationDuration * 320; i++)
            {
                clip.WriteSample((byte)Mathf.Clamp((int)(vibrationAmplitude * 255 * (1 + releaseLinVel.magnitude / 10)), 0, 255));
            }
            OVRHaptics.RightChannel.Preempt(clip);
            OVRHaptics.LeftChannel.Preempt(clip);

            NetEffectController.Instance.PlayStrikeZoneMissEffect(transform.position);
        }
    }

    void OnTriggerExit(Collider collider) {
        if (collider.tag.Equals("SceneBarrier")) {
            StopAllCoroutines();
            GlobalSettings.Selectable = true;
            Destroy(gameObject);
        }
    }


    // For NET
    public void OnHitByBat(Vector3 pos, Vector3 vel)
    {
        if (GlobalSettings.UseNetwork && MatchController.PlayerType == PlayerType.Batter)
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
