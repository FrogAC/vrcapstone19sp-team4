using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientAudioManager : MonoBehaviour
{ 
    public AudioClip[] cheers;
    public AudioClip ambientClip;
    public AudioSource ambient, ambientOffset, cheer, applause;
    [SerializeField]
    private int fadeInTime, cheerInterval, cheerStartDelay;

    private float ambientTrackLength, ambientTrackLoopOffset;

    // Start is called before the first frame update
    void Start()
    {
        ambientTrackLength = ambientClip.length;
        ambientTrackLoopOffset = ambientTrackLength / 2;
        InitAmbientNoiseTracks();
        StartAmbientNoiseTracks();
        cheer.loop = false;
        ThrownBall.OnHit += PlayHitApplause;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time <= fadeInTime)
        {
            ambient.volume = Time.time / fadeInTime * 0.5f;
            ambientOffset.volume = Time.time / fadeInTime * 0.5f;
        }

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

    private void OnEnable()
    {
        ThrownBall.OnHit += PlayHitApplause;
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

    public void PlayHitApplause()
    {
        if (!applause.isPlaying)
        {
            applause.loop = false;
            applause.Play();
            
        }
    }
}
