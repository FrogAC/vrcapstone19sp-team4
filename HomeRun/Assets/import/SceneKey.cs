using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneKey : MonoBehaviour {
	[SerializeField]
	int sceneIndex;


    [SerializeField]
    float delay =2f;

    [SerializeField]
    ParticleSystem particleSystem;


	public void beginLoading(){
		StartCoroutine (LoadEffects ());

	}
    
    public void LoadSceneFromObject(){
		SceneManager.LoadSceneAsync(sceneIndex);
	}


	public IEnumerator LoadEffects(){
        //stuff to look flashy
        if (particleSystem != null)
        {
            particleSystem.Play();
        }

        yield return new WaitForSecondsRealtime(delay);
		LoadSceneFromObject();
	}
}
