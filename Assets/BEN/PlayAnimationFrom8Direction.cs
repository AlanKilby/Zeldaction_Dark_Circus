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
    private bool _showRay; 
    
    // 0
    [Space, SerializeField] private List<AnimationClip> _fireClipList = new List<AnimationClip>(); // 1
    // 2
    // 3 
    
    // only for central cannon 
    void FixedUpdate() 
    {
        // do according to state to find right list
        if (!_showRay) return; 
        if (_position == Position.Center)
        {
            if (_rayManagerRotation.rotation.eulerAngles.y > 180f - _referenceAngleValue && 
                _rayManagerRotation.rotation.eulerAngles.y < 180f + _referenceAngleValue) // bot
            { 
                Debug.Log("CANON BOT");
                if (_animator.GetCurrentAnimatorStateInfo(0).IsName(_fireClipList[2].name)) return; 
                _animator.Play(_fireClipList[2].name);
            }
            else if (_rayManagerRotation.rotation.eulerAngles.y >= 180f - (_referenceAngleValue*2f) &&
                     _rayManagerRotation.rotation.eulerAngles.y <= 180f - _referenceAngleValue)  // 3-4 right 
            {
                Debug.Log("CANON 3-4 RIGHT");

                if (_animator.GetCurrentAnimatorStateInfo(0).IsName(_fireClipList[3].name)) return; 
                _animator.Play(_fireClipList[3].name);
            }
            else if (_rayManagerRotation.rotation.eulerAngles.y >= 180f + _referenceAngleValue &&
                     _rayManagerRotation.rotation.eulerAngles.y <= 180f + (_referenceAngleValue*2)) // 3-4 left 
            {
                Debug.Log("CANON 3-4 LEFT"); 

                if (_animator.GetCurrentAnimatorStateInfo(0).IsName(_fireClipList[1].name)) return; 
                _animator.Play(_fireClipList[1].name); 
            } 
        }
        else if (_position == Position.Left)
        {
            if (_rayManagerRotation.rotation.eulerAngles.y > 180f - _referenceAngleValue && 
                _rayManagerRotation.rotation.eulerAngles.y < 180f + _referenceAngleValue) // center is bot
            { 
                Debug.Log("CANON 3-4 LEFT"); 

                if (_animator.GetCurrentAnimatorStateInfo(0).IsName(_fireClipList[1].name)) return; 
                _animator.Play(_fireClipList[1].name); 
            } 
        }
        else if (_position == Position.Right) 
        {
            if (_rayManagerRotation.rotation.eulerAngles.y > 180f - _referenceAngleValue && 
                _rayManagerRotation.rotation.eulerAngles.y < 180f + _referenceAngleValue) // center is bot
            { 
                Debug.Log("CANON 3-4 RIGHT");

                if (_animator.GetCurrentAnimatorStateInfo(0).IsName(_fireClipList[3].name)) return; 
                _animator.Play(_fireClipList[3].name);
            }
        }
        // _referenceAngleValue*3
        
    }
    
    public void ShowRayVisuals(bool show)
    {
        _showRay = show;
    }
}
