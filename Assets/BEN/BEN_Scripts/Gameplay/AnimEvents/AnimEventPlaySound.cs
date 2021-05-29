using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using Sirenix.OdinInspector;

public enum SoundType { Walk, Attack, Hurt, Defend, Death } 

[RequireComponent(typeof(AudioSource))]
public class AnimEventPlaySound : SerializedMonoBehaviour
{
    private AudioSource _audioSource;
    public Dictionary<SoundType, List<Sound>> _soundsDictionary = new Dictionary<SoundType, List<Sound>>();
    private bool _settingFirstAudioClip;
    private AudioClip _currentClip, _newClip; 

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>(); 
    }

    public void PlaySoundSafe(SoundType type)
    {
        if (_audioSource.isPlaying) return; 
        _audioSource.PlayOneShot(_soundsDictionary[type][0].clip);  
     }

     public void PlayRandomSoundSafe(SoundType type) 
     {
         if (_audioSource.isPlaying) return;
         var selector = Random.Range(0, _soundsDictionary[type].Count); 
         
         /* if (!_settingFirstAudioClip)
         {
             _settingFirstAudioClip = true;
             _currentClip = _newClip = _soundsDictionary[type][selector].clip; 
         } => WIP to interrupt previous sound if new is different type (monkey_attack => monkey_death)*/
         
         _audioSource.PlayOneShot(_soundsDictionary[type][selector].clip); 
     }

     public void PlaySoundOverwrite(SoundType type) 
     {
         _audioSource.PlayOneShot(_soundsDictionary[type][0].clip);  
     }
     
     public void PlayRandomSoundOverwrite(SoundType type) 
     {
         var selector = Random.Range(0, _soundsDictionary[type].Count);
         _audioSource.PlayOneShot(_soundsDictionary[type][selector].clip); 
     }
     
}

[System.Serializable] 
public class Sound
{
    public AudioClip clip;
}
