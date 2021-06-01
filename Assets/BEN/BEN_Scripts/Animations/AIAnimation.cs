using System;
using BEN.AI;
using UnityEngine;

namespace BEN.Animation
{
    public enum AnimState
    {
        Idle,
        Walk,
        Atk,
        Hit,
        SecondaryAtk,
        Miss,
        Die 
    }

    public enum AnimDirection
    {
        None = -1,
        Top = 0, 
        Right = 1, 
        Bottom = 2, 
        Left = 3
    }

    [RequireComponent(typeof(Animator))]
    public class AIAnimation : MonoBehaviour
    {
        [SerializeField] private AIAnimationSO _animationSo;

        private Animator _animator;
        private AIType _type;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _animator.runtimeAnimatorController = _animationSo.controller;
        }

        public Clip PlayAnimation(AnimState key, AnimDirection direction)
        {
            var clipToPlay = _animationSo.GetAnimClipFromDictionary(key, direction); 
            
            if (!_animator) _animator = GetComponent<Animator>(); 
            if (!_animator.runtimeAnimatorController)
            {
                _animator.runtimeAnimatorController = _animationSo.controller;
            }

            if (!_animator.enabled)
                _animator.enabled = true;

            try
            {
                _animator.speed = clipToPlay.speedMultiplier; 

                if (_animator.GetCurrentAnimatorStateInfo(0).IsName(clipToPlay.clipContainer.name)) return null; 
                _animator.Play(clipToPlay.clipContainer.name);
            }
            catch (Exception) { }

            return clipToPlay;
        }
        
        public void PlayAnimationFromUnityEvent(AnimState key, AnimDirection direction)
        {
            var clipToPlay = _animationSo.GetAnimClipFromDictionary(key, direction); 
            
            if (!_animator) _animator = GetComponent<Animator>(); 
            if (!_animator.runtimeAnimatorController)
            {
                _animator.runtimeAnimatorController = _animationSo.controller;
            }

            if (!_animator.enabled)
                _animator.enabled = true;

            try
            {
                _animator.speed = clipToPlay.speedMultiplier; 

                if (_animator.GetCurrentAnimatorStateInfo(0).IsName(clipToPlay.clipContainer.name)) return; 
                _animator.Play(clipToPlay.clipContainer.name);
            }
            catch (Exception) { } 
        }

        public void StopAnimating()
        {
            _animator.enabled = false; 
        } 
    }
} 
