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
        [HideInInspector] public Animator animator;
        private AIType _type;

        private void Start()
        {
            animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = _animationSo.controller;
        }

        public Clip PlayAnimation(AnimState key, AnimDirection direction)
        {
            var clipToPlay = _animationSo.GetAnimClipFromDictionary(key, direction); 
            
            if (!animator) animator = GetComponent<Animator>(); 
            if (!animator.runtimeAnimatorController)
            {
                animator.runtimeAnimatorController = _animationSo.controller;
            }

            if (!animator.enabled)
                animator.enabled = true;

            try
            {
                animator.speed = clipToPlay.speedMultiplier; 

                if (animator.GetCurrentAnimatorStateInfo(0).IsName(clipToPlay.clipContainer.name)) return null; 
                animator.Play(clipToPlay.clipContainer.name);
            }
            catch (Exception) { }

            return clipToPlay;
        }
        
        public void PlayAnimationFromUnityEvent(AnimState key, AnimDirection direction)
        {
            var clipToPlay = _animationSo.GetAnimClipFromDictionary(key, direction); 
            
            if (!animator) animator = GetComponent<Animator>(); 
            if (!animator.runtimeAnimatorController)
            {
                animator.runtimeAnimatorController = _animationSo.controller;
            }

            if (!animator.enabled)
                animator.enabled = true;

            try
            {
                animator.speed = clipToPlay.speedMultiplier; 

                if (animator.GetCurrentAnimatorStateInfo(0).IsName(clipToPlay.clipContainer.name)) return; 
                animator.Play(clipToPlay.clipContainer.name);
            }
            catch (Exception) { } 
        } 

        public void StopAnimating()
        {
            animator.enabled = false; 
        } 
    }
} 
