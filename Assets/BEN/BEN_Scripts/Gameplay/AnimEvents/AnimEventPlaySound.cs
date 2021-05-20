using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEventPlaySound : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    public void PlaySound(AudioClip clip)
    {
        if (_audioSource.isPlaying || !_audioSource) return;
        _audioSource.PlayOneShot(clip);  
    }
}
