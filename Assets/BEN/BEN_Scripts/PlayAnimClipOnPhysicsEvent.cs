using System;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Animator))]
public class PlayAnimClipOnPhysicsEvent : MonoBehaviour
{
    [FormerlySerializedAs("_wallLayer")] [SerializeField] private LayerMask _callerLayer;
    [FormerlySerializedAs("_clip")] [SerializeField] private AnimationClip _clipToPlay;
    private Animator _animator; 

    private void Start()
    {
        _animator = GetComponent<Animator>(); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Mathf.Pow(other.gameObject.layer, 2f) != _callerLayer) return; 
        
        _animator.Play(_clipToPlay.name); 
    }
}
