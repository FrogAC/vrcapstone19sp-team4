using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombineScenes : MonoBehaviour {
    //made this a variable so is becomes a general solution
    //made it an array so it can load more than one scene using a loop
    [SerializeField] string[] scenes;

	// Use this for initialization
	void Start () {
        //moved to its own method, keep Start and Update limted to method calls as much as possible to keep them easily readable and managable;
        AdditvelyLoadScenes();
	}

    void AdditvelyLoadScenes()
    {
        
        foreach(string scene in scenes)
        {
            //checks if the scen is allready loaded, only loads if it has not been loaded.
            if (SceneManager.GetSceneByName(scene).isLoaded == false)
            {
                SceneManager.LoadScene(scene, LoadSceneMode.Additive);
            }
        }

    }


}
