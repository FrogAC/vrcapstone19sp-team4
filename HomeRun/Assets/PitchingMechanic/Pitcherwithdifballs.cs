using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;
using HomeRun;
using HomeRun.Net;
using HomeRun.Game;

public class Pitcherwithdifballs : MonoBehaviour
{
    public bool autoPitch;
    public float randomThrowRange = 1;
    public float randomThrowDelay = 3;
    [Space]

    public ThrownBall[] ballToThrow;
    public ThrownBall baseballPrefab;
    public float lifetime = 6;  // TODO remove
    public float speed = 5;
    System.Random random = new System.Random();

    [Space]
    //public Transform ballTarget;
    [Space]
    [SerializeField] OVRInput.Button nextSelection = OVRInput.Button.Three;
    [SerializeField] OVRInput.Button prevSelection = OVRInput.Button.Four;
    [SerializeField] OVRInput.Button throwBall = OVRInput.Button.One;

    int currIndex = 0;
    void OnEnable() {
        if (autoPitch)
        {
            StartCoroutine(AutomaticPitching());
        }
    }
    IEnumerator AutomaticPitching()
    {
        while (true)
        {
            yield return new WaitForSeconds(randomThrowDelay);

            transform.forward += new Vector3(Random.Range(-randomThrowRange, randomThrowRange) * 3, Random.Range(-randomThrowRange, randomThrowRange), 1);
            currIndex = Random.Range(0, ballToThrow.Length);
            ThrowBall();
        }
    }

    void ThrowBall()
    {
        baseballPrefab = ballToThrow[currIndex];
        ThrownBall ball = Instantiate(baseballPrefab, transform.position, transform.rotation);

        if (GlobalSettings.UseNetwork && PlatformManager.CurrentState == PlatformManager.State.PLAYING_A_NETWORKED_MATCH)
        {
            PlatformManager.P2P.AddNetworkBall(ball.gameObject, ball.balltype);
        }
        ball.GrabEnd(transform.forward * speed, Vector3.zero);
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(throwBall) || Input.GetKeyDown("space"))
        {// || OVRInput.GetDown(OVRInput.Button.Any, OVRInput.Controller.All)) {
            ThrowBall();
        }
        if (OVRInput.GetDown(nextSelection) || Input.GetKeyDown(KeyCode.E))
        {
            currIndex = ((currIndex + 1) + ballToThrow.Length) % ballToThrow.Length;
        }
        if (OVRInput.GetDown(prevSelection))
        {
            currIndex = ((currIndex - 1) + ballToThrow.Length) % ballToThrow.Length;
        }
    }

    private void FixedUpdate()
    {
        //transform.forward += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 1);
        //transform.Rotate(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"), 0, Space.World);
    }
}
