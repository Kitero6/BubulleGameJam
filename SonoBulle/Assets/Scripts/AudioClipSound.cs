using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioClipSound
{

    public List<AudioClip> Clips = null;
    [Range(0f, 1f)] public float Volume = 0f;

    public void PlayToSource(AudioSource audioSource)
    {
        if (Clips.Count == 0)
            return;
            
        int idx = Mathf.FloorToInt(Random.Range(0, Clips.Count - 1));
        
        audioSource.clip = Clips[idx];
        audioSource.volume = Volume;
    }
}
