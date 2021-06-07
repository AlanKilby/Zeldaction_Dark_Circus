using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ChangeSoundVolume : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private AudioMixer _audioMixer; 

    public void SetNewVolume()
    {
        _audioMixer.SetFloat("MasterVolume", _slider.value); 
    }
}
