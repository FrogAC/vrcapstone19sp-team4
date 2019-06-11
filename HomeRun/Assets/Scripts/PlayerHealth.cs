using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health;
    public Color safeColor;
    public Color notSafeColor;
    public GameObject hitNotifier;
    public GameEvent event1;
    public GameObject deadText;
    private bool dead = false;
    public GameObject Zombie;

    // Start is called before the first frame update
    private void Start()
    {
        safeColor = hitNotifier.GetComponent<Renderer>().material.color;
        deadText.SetActive(false);
    }

    private void Update()
    {
        if (!dead && health <= 0)
        {
            dead = true;
            deadText.SetActive(true);
            Zombie.GetComponent<ZombiesManager>().spawnFrequency = int.MaxValue;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Zombie"))
        {
            Debug.Log("Zombie Trigger Occured");
            health -= 1;
            playerGotHit();
            if (health <= 0) Debug.Log("Player Just Died");
        }
    }

    public void playerGotHit(){
        StartCoroutine("LerpColor");
        StartCoroutine("UnLerpColor");
    }
 public float duration = .35f; // This will be your time in seconds.
 public float smoothness = 0.02f; // This will determine the smoothness of the lerp. Smaller values are smoother. Really it's the time between updates.
 
 IEnumerator LerpColor()
 {
     float progress = 0; //This float will serve as the 3rd parameter of the lerp function.
     float increment = smoothness/duration; //The amount of change to apply.
     while(progress < 1)
     {
        hitNotifier.GetComponent<Renderer>().material.color = Color.Lerp(safeColor, notSafeColor, progress);
        progress += increment;
        yield return new WaitForSeconds(smoothness);
     }
 }
IEnumerator UnLerpColor()
 {
     float progress = 0; //This float will serve as the 3rd parameter of the lerp function.
     float increment = smoothness/duration; //The amount of change to apply.
     while(progress < 1)
     {
        hitNotifier.GetComponent<Renderer>().material.color = Color.Lerp(notSafeColor, safeColor, progress);
        progress += increment;
        yield return new WaitForSeconds(smoothness);
     }
 }

    


}
