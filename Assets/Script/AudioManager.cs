using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource clickMusic;
    public AudioSource destroyMusic;
    
    public void PlayClickMusic()
    {
        if (clickMusic != null)
        {
            clickMusic.Play();
        }
    }

    public void PlayDestroyMusic()
    {
        if (destroyMusic != null && !destroyMusic.isPlaying)
        {
            destroyMusic.Play();
        }
    }
}
