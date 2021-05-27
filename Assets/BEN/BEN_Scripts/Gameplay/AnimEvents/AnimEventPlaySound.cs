using UnityEngine;

public class AnimEventPlaySound : MonoBehaviour
{
    [SerializeField] private Sound _soundToPlay;

    public void PlaySound() 
    {
        if (_soundToPlay.audioSource.isPlaying || !_soundToPlay.audioSource) return;
        _soundToPlay.audioSource.PlayOneShot(_soundToPlay.clip);  
    } 
}

[System.Serializable] 
public class Sound
{
    public string name; 
    public AudioClip clip;
    public AudioSource audioSource; 
}
