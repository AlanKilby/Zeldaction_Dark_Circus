using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using Sirenix.OdinInspector;

public enum SoundType { Walk, Attack, Hurt, Defend, Death, Laugh, Vulnerable, PreJump, Reset } 

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
        _audioSource.PlayOneShot(_soundsDictionary[type].Count == 1 ? _soundsDictionary[type][0].clip : 
                                                                      _soundsDictionary[type][Random.Range(0, _soundsDictionary[type].Count)].clip);  
    } 

     public void PlaySoundOverwrite(SoundType type) 
     {
         _audioSource.PlayOneShot(_soundsDictionary[type].Count == 1 ? _soundsDictionary[type][0].clip : 
                                                                       _soundsDictionary[type][Random.Range(0, _soundsDictionary[type].Count)].clip);  
     } 
}

[Serializable] 
public class Sound
{
    public AudioClip clip;
}
