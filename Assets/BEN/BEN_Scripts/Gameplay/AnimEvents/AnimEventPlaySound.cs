using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using Sirenix.OdinInspector;
using UnityEngine.Audio;

public enum SoundType { Walk, Attack, Hurt, Defend, Death, Laugh, Vulnerable, PreJump, Reset } 

[RequireComponent(typeof(AudioSource))]
public class AnimEventPlaySound : SerializedMonoBehaviour
{
    private AudioSource _audioSource;
    public Dictionary<SoundType, List<Sound>> _soundsDictionary = new Dictionary<SoundType, List<Sound>>();
    private bool _settingFirstAudioClip;
    private Sound _sound;
    private bool hasBeenPlayed; 

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>(); 
    }

    public void PlaySoundSafe(SoundType type)
    {
        if (_audioSource.isPlaying) return;
        _sound = _soundsDictionary[type].Count == 1 ? 
            _soundsDictionary[type][0] : 
            _soundsDictionary[type][Random.Range(0, _soundsDictionary[type].Count)];
        
        _audioSource.outputAudioMixerGroup =  _sound.Group ? _sound.Group : _audioSource.outputAudioMixerGroup; 
        _audioSource.PlayOneShot(_sound.clip);  
    } 

     public void PlaySoundOverwrite(SoundType type) 
     {
         _sound = _soundsDictionary[type].Count == 1 ? 
             _soundsDictionary[type][0] : 
             _soundsDictionary[type][Random.Range(0, _soundsDictionary[type].Count)];
         
         _audioSource.outputAudioMixerGroup =  _sound.Group ? _sound.Group : _audioSource.outputAudioMixerGroup; 
         _audioSource.PlayOneShot(_sound.clip);   
     }

     public void PlaySoundSafeOnce(SoundType type)
     {
         if (_audioSource.isPlaying || hasBeenPlayed) return;
         hasBeenPlayed = true; 
         _sound = _soundsDictionary[type].Count == 1 ? 
             _soundsDictionary[type][0] : 
             _soundsDictionary[type][Random.Range(0, _soundsDictionary[type].Count)];
        
         _audioSource.outputAudioMixerGroup =  _sound.Group ? _sound.Group : _audioSource.outputAudioMixerGroup; 
         _audioSource.PlayOneShot(_sound.clip);  
     }
     
     public void PlaySoundOverwriteOnce(SoundType type)
     {
         if (hasBeenPlayed) return; 
         hasBeenPlayed = true;
         _sound = _soundsDictionary[type].Count == 1 ? 
             _soundsDictionary[type][0] : 
             _soundsDictionary[type][Random.Range(0, _soundsDictionary[type].Count)];
         
         _audioSource.outputAudioMixerGroup =  _sound.Group ? _sound.Group : _audioSource.outputAudioMixerGroup; 
         _audioSource.PlayOneShot(_sound.clip);  
     }
} 

[Serializable] 
public class Sound 
{
    public AudioClip clip;
    [SerializeField] AudioMixerGroup _group;

    public AudioMixerGroup Group { get => _group; }

}
