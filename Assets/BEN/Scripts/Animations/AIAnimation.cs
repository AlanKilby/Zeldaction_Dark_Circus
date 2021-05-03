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
        Die 
    }

    public enum AnimDirection
    {
        None = -1,
        Top, 
        Right, 
        Bottom, 
        Left
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
            _animationSo.PopulateDictionary(); 
        }

        /* public void PlayAnimation(AnimState clip)
        {
            if (!_animator) _animator = GetComponent<Animator>(); 
            if (!_animator.runtimeAnimatorController)
            {
                _animator.runtimeAnimatorController = _animationSo.controller;
            }

            if (!_animator.enabled)
                _animator.enabled = true;  

            _animator.speed = _animationSo.clipListArray[(int)clip].speedMultiplier;

            if (_animator.GetCurrentAnimatorStateInfo(0).IsName(_animationSo.clipListArray[(int) clip].clipContainer.name)) return;
            _animator.Play(_animationSo.clipListArray[(int)clip].clipContainer.name);
        } */
        
        /* public void PlayAnimation(int animIndex)
        {
            if (!_animator) return;
            if (!_animator.runtimeAnimatorController) 
            {
                _animator.runtimeAnimatorController = _animationSo.controller;
            }

            if (!_animator.enabled)
                _animator.enabled = true; 

            _animator.speed = _animationSo.clipListArray[animIndex].speedMultiplier;

            if (_animator.GetCurrentAnimatorStateInfo(0).IsName(_animationSo.clipListArray[animIndex].clipContainer.name)) return;
            _animator.Play(_animationSo.clipListArray[animIndex].clipContainer.name);
        } */

        public void StopAnimating()
        {
            _animator.enabled = false; 
        } 
    }
} 
