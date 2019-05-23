using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;

public class PlayerUI : MonoBehaviour
{
    public OVRInput.Button menuButton = OVRInput.Button.Start;
    public Vector3 offset = Vector3.forward;
    public Transform headset;
    public GameObject menuParent;

    private bool menuVisible = false;

    // Start is called before the first frame update
    void Start()
    {
        menuParent.SetActive(menuVisible);
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(menuButton))
        {
            if (menuVisible)
            {

            } else
            {
                Vector3 bodyForward = Vector3.ProjectOnPlane(headset.transform.forward, Vector3.up);
                transform.position = headset.transform.position + Vector3.up * offset.y + bodyForward * offset.z;
                //transform.position = headset.transform.position + headset.transform.TransformVector(offset) - Vector3.up * 0.3f;
                transform.LookAt(headset);
            }

            menuVisible = !menuVisible;
            menuParent.SetActive(menuVisible);
        }
    }
}
