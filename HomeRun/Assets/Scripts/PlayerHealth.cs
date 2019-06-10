using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health;
    public Color safeColor;
    public Color notSafeColor;
    public GameObject hitNotifier;

    // Start is called before the first frame update
    private void Start()
    {
        safeColor = hitNotifier.GetComponent<Renderer>().material.color;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Zombie"))
        {
            Debug.Log("Zombie Trigger Occured");
            health -= 1;
            StartCoroutine("changeColor");
            if (health <= 0) Debug.Log("Player Just Died");
        }
    }

    IEnumerator changeColor()
    {
        hitNotifier.GetComponent<Renderer>().material.color = Color.Lerp(safeColor, notSafeColor, 5.0f);
        yield return null;
       // hitNotifier.GetComponent<Renderer>().material.color = Color.Lerp(notSafeColor, safeColor, 1.0f);
    }


}
