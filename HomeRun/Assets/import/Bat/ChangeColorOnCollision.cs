using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColorOnCollision : MonoBehaviour
{
    [SerializeField]
    int init_X_Velocity_On_Collision;
    [SerializeField]
    int init_Y_Velocity_On_Collision;
    [SerializeField]
    int init_Z_Velocity_On_Collision;

    //Used to track the final velocity prior to a collision
    //float previousY;

    //Update is called once per frame
    //void Update()
    //{
    //    if (GetComponent<Rigidbody>().velocity.y != 0) previousY = -(GetComponent<Rigidbody>().velocity.y);
    //}

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name.Equals("fastball(Clone)") || collision.gameObject.name.Equals("curveball(Clone)") || collision.gameObject.name.Equals("spiralball(Clone)"))
        {
            collision.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(init_X_Velocity_On_Collision, init_Y_Velocity_On_Collision, init_Z_Velocity_On_Collision);
        }
    }
}
