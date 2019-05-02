using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Strikezone : MonoBehaviour
{
    public static Strikezone strikezone;
    private void Awake()
    {
        if (strikezone == null)
        {
            strikezone = this;
        } else
        {
            Debug.LogError("Strikezone singleton already assigned");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
