using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientAudioManager : MonoBehaviour
{ 
    public AudioClip[] cheers;
    public AudioClip ambientClip;
    public AudioSource ambient, ambientOffset, cheer;
    [SerializeField]
    private int cheerInterval, cheerStartDelay;

    private float ambientTrackLength, ambientTrackLoopOffset;

    // Start is called before the first frame update
    void Start()
    {
        ambientTrackLength = ambientClip.length;
        ambientTrackLoopOffset = ambientTrackLength / 2;
        InitAmbientNoiseTracks();
        StartAmbientNoiseTracks();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > cheerStartDelay && (int)Time.time % cheerInterval == 0)
        {
            if (!cheer.isPlaying)
            {
                cheer.clip = cheers[Random.Range(0, cheers.Length)];
                cheer.loop = false;
                cheer.Play();
            }
        }
        
    }



    private void InitAmbientNoiseTracks()
    {
        ambient.clip = ambientClip;
        ambientOffset.clip = ambientClip;
        ambientOffset.time = ambientTrackLoopOffset;
        ambient.loop = ambientOffset.loop = true;
        
    }

    public void StartAmbientNoiseTracks()
    {
        ambient.Play();
        ambientOffset.Play();
    }
}
