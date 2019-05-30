using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendCollision : MonoBehaviour
{
    public GameObject[] targetObjects;
    public void OnCollisionEnter(Collision collision)
    {
        foreach(GameObject obj in targetObjects) {
            if (obj != null){
                obj.SendMessage("OnCollisionEnter", collision, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    public void OnCollisionStay(Collision collision)
    {
        foreach(GameObject obj in targetObjects) {
            if (obj != null){
                obj.SendMessage("OnCollisionStay", collision, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
    public void OnCollisionExit(Collision collision)
    {
        foreach(GameObject obj in targetObjects) {
            if (obj != null){
                obj.SendMessage("OnCollisionExit", collision, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        foreach(GameObject obj in targetObjects) {
            if (obj != null){
                obj.SendMessage("OnTriggerEnter", other, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    public void OnTriggerStay(Collider other)
    {
        foreach(GameObject obj in targetObjects) {
            if (obj != null){
                obj.SendMessage("OnTriggerStay", other, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        foreach(GameObject obj in targetObjects) {
            if (obj != null){
                obj.SendMessage("OnTriggerExit", other, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}
