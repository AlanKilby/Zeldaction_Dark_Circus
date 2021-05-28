using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using Sirenix.OdinInspector;

public enum SoundType { Attack, Hurt, Defend, Death } 

[RequireComponent(typeof(AudioSource))]
public class AnimEventPlaySound : SerializedMonoBehaviour
{
    private AudioSource _audioSource;
    public Dictionary<SoundType, List<Sound>> _soundsDictionary = new Dictionary<SoundType, List<Sound>>();

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>(); 
    }

    public void PlaySound(SoundType type) 
     {
         _audioSource.PlayOneShot(_soundsDictionary[type][0].clip);  
     }

     public void PlayRandomSound(SoundType type) 
     {
         var selector = Random.Range(0, _soundsDictionary[type].Count); 
         _audioSource.PlayOneShot(_soundsDictionary[type][selector].clip);  
     }
     
}

[System.Serializable] 
public class Sound
{
    public string name; 
    public AudioClip clip;
}
