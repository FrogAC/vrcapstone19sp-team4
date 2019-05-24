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

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 12)
        {
            Debug.Log("Homerun");
            manager.HomeRun();
        }
    }
}
