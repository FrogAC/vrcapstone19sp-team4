using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldBoundary : MonoBehaviour
{
    private HRGameManager manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<HRGameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "ball")
        {
            manager.HomeRun();
        }
    }
}
