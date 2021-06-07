using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public class FallDetection : MonoBehaviour
{
    [SerializeField] private LayerMask _groundLayer, _playerLayer; 
    [SerializeField] private UnityEvent _OnGroundDetection, _PlayAnimationOnGroundHit;
    [SerializeField] private GameObject _destructionEffect;
    [SerializeField] private bool destroyOnHit; 
    [SerializeField, Range(1, 5)] private sbyte _damage = 1;
    public static Action OnGroundDetection; // destroy other prefab (shadow)
    
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _audioClip; 
    [SerializeField] private AudioMixerGroup _group;

    public static Action<int> NotifyShadowOnReachingGround;
    public int Index { get; set; }
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (Mathf.Pow(2f, other.gameObject.layer) == _groundLayer)
        { 
            try
            {
                NotifyShadowOnReachingGround(Index);
            }
            catch (Exception) { }
            
            if (_audioSource.outputAudioMixerGroup.name != "Projo crash") 
            {
                _audioSource.outputAudioMixerGroup = _group; 
                _audioSource.PlayOneShot(_audioClip);
            }
            
            GetComponent<BoxCollider>().isTrigger = false; 

            if (destroyOnHit)
            {
                _OnGroundDetection.Invoke(); // play sound and destroy self
                BossEventProjectileFalling.DestroyShadowAtIndex(Index, 2f); 
            }

            _PlayAnimationOnGroundHit.Invoke(); 
        } 
        
        if (Mathf.Pow(2f, other.gameObject.layer) == _playerLayer)
        {
            _audioSource.outputAudioMixerGroup = _group;
            _audioSource.PlayOneShot(_audioClip);
            
            other.GetComponent<Health>().DecreaseHp(_damage); 
            if (destroyOnHit)
            { 
                _OnGroundDetection.Invoke(); // play sound and destroy self
                BossEventProjectileFalling.DestroyShadowAtIndex(Index, 2f); 
            } 
        }
    } 

    private void OnDestroy()
    {
        if (!Application.isPlaying) return; 
        Instantiate(_destructionEffect, new Vector3(transform.position.x, 1.2f, transform.position.z), Quaternion.identity);
    }
}
