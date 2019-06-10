using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAudioManager : MonoBehaviour
{
    private AudioSource src;
    private int offset;
    public int frequency;
    public AudioClip[] clips;

    // Start is called before the first frame update
    void Start()
    {
        src = gameObject.GetComponent<AudioSource>();
        src.clip = clips[Random.Range(0, clips.Length - 1)];
        offset = Random.Range(0, 3);
        src.loop = src.playOnAwake = false;
    }

    // Update is called once per frame
    void Update()
    {
        if ((int)Time.time % (frequency + offset) == 0 && !src.isPlaying)
        {
            if (Random.Range(0, 9) < 3)
            {
                src.Play();
            }
        }
    }
}
