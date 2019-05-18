using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class VRButton : MonoBehaviour
{
    public UnityEvent onPressedEvent;
    public float activationDelay = 1;
    public Color activatedColor = Color.cyan;
    private bool isPressed;

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
        if (other.tag == "Hand" && !isPressed)
        {
            isPressed = true;
            StartCoroutine(ButtonActivationDelay());
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Hand" && isPressed)
        {
            isPressed = false;
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

    IEnumerator ButtonActivationDelay()
    {
        Renderer ren = GetComponent<Renderer>();
        Color startColor = ren.material.color;
        float startTime = Time.time;

        while (Time.time < startTime + activationDelay && isPressed)
        {
            float t = (Time.time - startTime) / activationDelay;
            ren.material.color = Color.Lerp(startColor, activatedColor, t);
            yield return null;
        }

        if (isPressed)
        {
            onPressedEvent.Invoke();
        }
        yield return null;
        ren.material.color = startColor;

        
    }
}
