using System;
using BEN.Scripts.FSM;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BEN.Scripts
{
    public enum AnimationState
    {
        IdleRight = 0,
        IdleLeft, 
        Hit,
        WalkTop,
        WalkRight, 
        WalkBot, 
        WalkLeft, 
        AtkTop,
        AtkRight,
        AtkBot, 
        AtkLeft 
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

        public void SetType(AIType type) // called on Awake or from Editor
        {
            _type = type;
            _animationSo = GameManager.Instance.scriptableAnimationList[(int) _type]; 
        }

        public void PlayAnimation(AnimationState clip)
        {
            if (!_animator) _animator = GetComponent<Animator>(); 
            if (!_animator.runtimeAnimatorController)
            {
                _animator.runtimeAnimatorController = _animationSo.controller;
            }

            if (!_animator.enabled)
                _animator.enabled = true;  

            _animator.speed = _animationSo.clipList[(int)clip].speedMultiplier;

            if (_animator.GetCurrentAnimatorStateInfo(0).IsName(_animationSo.clipList[(int) clip].clipContainer.name)) return;
            _animator.Play(_animationSo.clipList[(int)clip].clipContainer.name);
        }
        
        public void PlayAnimation(int animIndex)
        {
            if (!_animator) return;
            if (!_animator.runtimeAnimatorController) 
            {
                _animator.runtimeAnimatorController = _animationSo.controller;
            }

            if (!_animator.enabled)
                _animator.enabled = true; 

            _animator.speed = _animationSo.clipList[animIndex].speedMultiplier;

            if (_animator.GetCurrentAnimatorStateInfo(0).IsName(_animationSo.clipList[animIndex].clipContainer.name)) return;
            _animator.Play(_animationSo.clipList[animIndex].clipContainer.name);
        }

        public void StopAnimating()
        {
            _animator.enabled = false; 
        } 
    }
} 
