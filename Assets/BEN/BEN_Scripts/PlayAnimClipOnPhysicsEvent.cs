using System;
using System.Collections.Generic; 
using UnityEngine;
using UnityEngine.Serialization;

public class PlayAnimClipOnPhysicsEvent : MonoBehaviour
{
    [FormerlySerializedAs("_wallLayer")] [SerializeField] private LayerMask _callerLayer;
    [FormerlySerializedAs("_clip")] [SerializeField] private AnimationClip _clipToPlay;
    [SerializeField] private Animator _animator;
    [Space, SerializeField] private List<Behaviour> _behaviourToStopOnPhysicsEvent = new List<Behaviour>();
    [SerializeField] private bool _setNewRotationOnPhysicsEvent = true;  
    [SerializeField, ConditionalShow("setNewRotationOnPhysicsEvent", true)] private Vector3 _newRotation;
    [SerializeField, ConditionalShow("setNewRotationOnPhysicsEvent", true)] private GameObject _target;

    private void OnTriggerEnter(Collider other)
    {
        if (Mathf.Pow(2f, other.gameObject.layer) != _callerLayer) return;
        Debug.Log("detecting wall");
        _animator.Play(_clipToPlay.name);

        if (_behaviourToStopOnPhysicsEvent.Count == 0) return; 
        foreach (var t in _behaviourToStopOnPhysicsEvent)
        {
            t.enabled = false;
        } 

        if (!_setNewRotationOnPhysicsEvent) return; 
        _target.transform.rotation = Quaternion.Euler(_newRotation.x, _newRotation.y, _newRotation.z); 
    } 
}
