using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BEN.Scripts
{
    public enum AnimationState
    {
        IdleRight,
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
        [Range(0f, 2f)] public float speedMultiplier = 1f; 

        [Header("-- DEBUG --")]
        public AnimationClip animToPlay;  

        private void Start()
        {
            _animator = GetComponent<Animator>();
            animToPlay = _animationSo.clipList[0].clipContainer; 
        } 

        private void Update()
        {
            try
            {
                if (Input.GetKeyDown(KeyCode.P) && !_animator.GetCurrentAnimatorStateInfo(0).IsName(animToPlay.name))
                    PlayDebugAnim(animToPlay);  
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        private void PlayDebugAnim(AnimationClip clip)
        {
            _animator.Play(clip.name);
            _animator.speed = speedMultiplier; 
        } 

        public void PlayAnimation(AnimationState animationState) 
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName(animToPlay.name)) return;
            _animator.Play(_animationSo.clipList[(int)animationState].clipContainer.name); 
        }
    }
} 
