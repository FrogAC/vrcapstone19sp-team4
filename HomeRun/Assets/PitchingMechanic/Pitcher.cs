using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;

public class Pitcher : MonoBehaviour
{
	public ThrownBall baseballPrefab;
	public float lifetime = 6;
	public float speed = 5;

	[Space]
	public Transform ballTarget;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void ThrowBall() {
        ThrownBall ball = Instantiate(baseballPrefab, transform.position, transform.rotation);
        ball.initialize();
        ball.GrabEnd(transform.forward * speed, Vector3.zero);
        Destroy(ball, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space")){// || OVRInput.GetDown(OVRInput.Button.Any, OVRInput.Controller.All)) {
			ThrowBall();
		}
    }

	private void FixedUpdate()
	{
        transform.forward += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 1);
        //transform.Rotate(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"), 0, Space.World);
    }
}
