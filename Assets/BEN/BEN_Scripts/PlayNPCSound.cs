using System.Collections.Generic;
using UnityEngine;

public enum NPC { None = -1, Beastmaster, Canneline, Grobourdin, JeanMarc, Rothilda, Zavatta = 10 }
public class PlayNPCSound : MonoBehaviour
{
    public AudioSource _audioSource; 
    public NPC _npc; 
    public List<Sound> _soundsList = new List<Sound>();
    int soundIndex; 

    public void PlaySound()
    {
        if (_audioSource.isPlaying) return; 
        
        _audioSource.outputAudioMixerGroup = _soundsList[(int) _npc].Group;
        soundIndex = _npc switch
        {
            NPC.JeanMarc => Random.Range(3, 7),
            NPC.Rothilda => Random.Range(7, 10),
            _ => (int) _npc
        };
        
        _audioSource.PlayOneShot(_soundsList[soundIndex].clip); 
    } 
}
