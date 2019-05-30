using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health;
    // Start is called before the first frame update
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Zombie"))
        {
            health -= 1;
            if (health <= 0) Debug.Log("Player Just Died");
        }
    }
}
