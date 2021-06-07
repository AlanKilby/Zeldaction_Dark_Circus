using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State { Spawn, Fire, Despawn, PreFire}
public enum Directions {Left, TroisQuartLeft, Bot, TroisQuartRight, Right }
public enum Position {Left, Center, Right }
public class PlayAnimationFrom8Direction : MonoBehaviour
{
    [SerializeField] private Position _position; 
    [SerializeField] private Animator _animator; 
    [SerializeField] private Transform _rayManagerRotation;
    [SerializeField, Range(10f, 30f)] private float _referenceAngleValue = 20;
    private bool _setCanonAnimation; 

    [Space, SerializeField] private List<AnimationClip> _fireClipList = new List<AnimationClip>(); // only need entry point
    private static readonly int FireDone = Animator.StringToHash("fireDone");

    private void OnEnable()
    {
        RayAttack.OnFireDone += SetFireDoneDecorator; 
    }

    private void OnDisable()
    {
        RayAttack.OnFireDone -= SetFireDoneDecorator;
    }

    void FixedUpdate()
    {
        if (!_setCanonAnimation) return;
        Debug.Log("set canon animation is true");
        if (_position == Position.Center)
        { 
            if (_rayManagerRotation.rotation.eulerAngles.y > 180f - _referenceAngleValue && 
                _rayManagerRotation.rotation.eulerAngles.y < 180f + _referenceAngleValue) // bot
            { 
                if (_animator.GetCurrentAnimatorStateInfo(0).IsName(_fireClipList[2].name)) return; 
                _animator.Play(_fireClipList[2].name);
            }
            else if (_rayManagerRotation.rotation.eulerAngles.y >= 180f - (_referenceAngleValue*2f) &&
                     _rayManagerRotation.rotation.eulerAngles.y <= 180f - _referenceAngleValue)  // 3-4 right 
            {
                if (_animator.GetCurrentAnimatorStateInfo(0).IsName(_fireClipList[3].name)) return; 
                _animator.Play(_fireClipList[3].name);
            }
            else if (_rayManagerRotation.rotation.eulerAngles.y >= 180f + _referenceAngleValue &&
                     _rayManagerRotation.rotation.eulerAngles.y <= 180f + (_referenceAngleValue*2)) // 3-4 left 
            {
                if (_animator.GetCurrentAnimatorStateInfo(0).IsName(_fireClipList[1].name)) return; 
                _animator.Play(_fireClipList[1].name); 
            } 
        }
        else if (_position == Position.Left)
        {
            if (_rayManagerRotation.rotation.eulerAngles.y > 180f - _referenceAngleValue && 
                _rayManagerRotation.rotation.eulerAngles.y < 180f + _referenceAngleValue) // center is bot
            {
                if (_animator.GetCurrentAnimatorStateInfo(0).IsName(_fireClipList[1].name)) return; 
                _animator.Play(_fireClipList[1].name); 
            } 
            else if (_rayManagerRotation.rotation.eulerAngles.y >= 180f - (_referenceAngleValue*2f) &&
                     _rayManagerRotation.rotation.eulerAngles.y <= 180f - _referenceAngleValue)  // center is 3-4 right 
            {
                if (_animator.GetCurrentAnimatorStateInfo(0).IsName(_fireClipList[2].name)) return; 
                _animator.Play(_fireClipList[2].name); 
            }
            else if (_rayManagerRotation.rotation.eulerAngles.y >= 180f + _referenceAngleValue &&
                     _rayManagerRotation.rotation.eulerAngles.y <= 180f + (_referenceAngleValue*2)) // center is 3-4 left 
            {
                if (_animator.GetCurrentAnimatorStateInfo(0).IsName(_fireClipList[1].name)) return; 
                _animator.Play(_fireClipList[1].name); 
            } 
        }
        else if (_position == Position.Right) 
        {
            if (_rayManagerRotation.rotation.eulerAngles.y > 180f - _referenceAngleValue && 
                _rayManagerRotation.rotation.eulerAngles.y < 180f + _referenceAngleValue) // center is bot
            {
                if (_animator.GetCurrentAnimatorStateInfo(0).IsName(_fireClipList[3].name)) return; 
                _animator.Play(_fireClipList[3].name);
            }
            else if (_rayManagerRotation.rotation.eulerAngles.y >= 180f - (_referenceAngleValue*2f) &&
                     _rayManagerRotation.rotation.eulerAngles.y <= 180f - _referenceAngleValue)  // center is 3-4 right 
            {
                if (_animator.GetCurrentAnimatorStateInfo(0).IsName(_fireClipList[3].name)) return; 
                _animator.Play(_fireClipList[3].name);
            }
            else if (_rayManagerRotation.rotation.eulerAngles.y >= 180f + _referenceAngleValue &&
                     _rayManagerRotation.rotation.eulerAngles.y <= 180f + (_referenceAngleValue*2)) // center is 3-4 left 
            { 
                if (_animator.GetCurrentAnimatorStateInfo(0).IsName(_fireClipList[2].name)) return; 
                _animator.Play(_fireClipList[2].name); 
            } 
        } 
        // _referenceAngleValue*3
    } 
    
    public void ShowRayVisuals(bool show)
    {
        Debug.Log("setting show ray visuals to " + show);
        _setCanonAnimation = show;
    }

    private void SetFireDoneDecorator()
    {
        StartCoroutine(nameof(SetFireDone)); 
    } 

    private IEnumerator SetFireDone()
    { 
        _animator.SetBool(FireDone, true);
        Debug.Log("FIRE DONE TRUE");
        yield return new WaitForSeconds(0.5f);  
        Debug.Log("FIRE DONE FALSE");
        _animator.SetBool(FireDone, false);
    }
}
