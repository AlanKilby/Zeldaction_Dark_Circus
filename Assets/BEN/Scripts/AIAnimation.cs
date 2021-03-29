using System;
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

        private void OnValidate()
        {
            _requiredState = animToPlay;
            
            if (_requiredState == _currentState) return;
            PlayAnimation(animToPlay);
            _currentState = _requiredState;
        } 

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _currentState = _requiredState = animToPlay; 
        }

        private void Update() 
        {
            if (!refreshSpeed) return;
            _animator.speed = _animationSo.clipList[(int) animToPlay].speedMultiplier;
            refreshSpeed = false;

        }

        public void PlayAnimation(AnimationState clip)
        {
            if (!_animator) return; 
            _animator.speed = _animationSo.clipList[(int)clip].speedMultiplier;

            if (_animator.GetCurrentAnimatorStateInfo(0).IsName(_animationSo.clipList[(int) clip].clipContainer.name)) return;
            _animator.Play(_animationSo.clipList[(int)clip].clipContainer.name);
        }
    }
} 
