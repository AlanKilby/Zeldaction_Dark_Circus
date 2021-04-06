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
        
        [Header("-- DEBUG --")]
        public AnimationState animToPlay;

        private AnimationState _currentState;
        private AnimationState _requiredState;
        public bool refreshSpeed;
        private AIType _type; 

        private void OnValidate()
        {
            _requiredState = animToPlay;
            
            if (_requiredState == _currentState) return;
            PlayAnimation(animToPlay);
            _currentState = _requiredState;
        }
        
        public void SetType(AIType type) // called on Awake or from Editor
        {
            _type = type;
            _animationSo = GameManager.Instance.scriptableAnimationList[(int) _type]; 
        }

        private void Start()
        { 
            _animator = GetComponent<Animator>();
            _animator.runtimeAnimatorController = _animationSo.controller; 
            _currentState = _requiredState = animToPlay; 
        }

        private void Update() 
        {
            if (!refreshSpeed) return;
            _animator.speed = _animationSo.clipList[(int) animToPlay].speedMultiplier;
            _animator.runtimeAnimatorController = _animationSo.controller; 
            refreshSpeed = false;

        }

        public void PlayAnimation(AnimationState clip)
        {
            if (!_animator) return; 
            if (!_animator.runtimeAnimatorController)
            {
                _animator.runtimeAnimatorController = _animationSo.controller;
            }

            if (!_animator.enabled)
                _animator.enabled = true; 

            if (!_animator) return; 
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

        public void SetScriptable(AIAnimationSO scriptableObject)
        {
            Debug.Log("setting scriptable animation");
            _animationSo = scriptableObject;   
        }
    }
} 
