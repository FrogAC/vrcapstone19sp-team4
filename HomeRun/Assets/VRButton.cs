using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class VRButton : MonoBehaviour
{
    public UnityEvent onPressedOrTriggeredEvent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnTriggerEnter(collision.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hand")
        {
            onPressedOrTriggeredEvent.Invoke();
        }
    }

    public void ReloadScene()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }

    public void LoadScene(string name)
    {
        SceneManager.LoadSceneAsync(name);
    }
}
