using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioClipSound
{
    public AudioClip Clip = null;
    [Range(0f, 1f)] public float Volume = 0f;

    public void PlayToSource(AudioSource audioSource)
    {
        audioSource.clip = Clip;
        audioSource.volume = Volume;
    }
}
