using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health;
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Zombie"))
        {
            Debug.Log("Zombie Trigger Occured");
            health -= 1;
            if (health <= 0) Debug.Log("Player Just Died");
        }
    }
}
