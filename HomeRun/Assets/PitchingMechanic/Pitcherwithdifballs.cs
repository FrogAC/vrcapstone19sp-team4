using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;

public class Pitcherwithdifballs : MonoBehaviour
{
    public ThrownBall [] ballToThrow;
    public ThrownBall baseballPrefab;
    public float lifetime = 6;
    public float speed = 5;
    System.Random random = new System.Random();

    [Space]
    public Transform ballTarget;
    [Space]
    [SerializeField] OVRInput.Button nextSelection = OVRInput.Button.Three;
    [SerializeField] OVRInput.Button prevSelection = OVRInput.Button.Four;
    [SerializeField] OVRInput.Button throwBall = OVRInput.Button.One;

    int currIndex = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    void ThrowBall()
    {
        baseballPrefab = ballToThrow[currIndex];
        ThrownBall ball = Instantiate(baseballPrefab, transform.position, transform.rotation);
        ball.initialize();
        ball.GrabEnd(transform.forward * speed, Vector3.zero);
        Destroy(ball, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(throwBall) || Input.GetKeyDown("space"))
        {// || OVRInput.GetDown(OVRInput.Button.Any, OVRInput.Controller.All)) {
            ThrowBall();
        }
        if (OVRInput.GetDown(nextSelection))
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
        transform.forward += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 1);
        //transform.Rotate(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"), 0, Space.World);
    }
}
