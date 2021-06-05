using System;
using UnityEngine;
using UnityEngine.Events;

public class FallDetection : MonoBehaviour
{
    [SerializeField] private LayerMask _groundLayer, _playerLayer; 
    [SerializeField] private UnityEvent _OnGroundDetection;
    [SerializeField] private GameObject _destructionEffect;
    [SerializeField, Range(1, 5)] private sbyte _damage = 1;
    public static Action OnGroundDetection; // destroy other prefab (shadow)
    
    private void OnTriggerEnter(Collider other)
    {
        // refactor to avoid duplicate code !
        if (Mathf.Pow(2f, other.gameObject.layer) == _groundLayer)
        { 
            // Debug.Log("ground");
            _OnGroundDetection.Invoke(); // play sound and destroy self
            OnGroundDetection(); 
        } 
        else if (Mathf.Pow(2f, other.gameObject.layer) == _playerLayer)
        {
            // Debug.Log("player"); 
            other.GetComponent<Health>().DecreaseHp(_damage); 
            _OnGroundDetection.Invoke(); // play sound and destroy self
            OnGroundDetection(); 
        }
    } 

    private void OnDestroy()
    {
        if (!Application.isPlaying) return; 
        Instantiate(_destructionEffect, new Vector3(transform.position.x, 1.2f, transform.position.z), Quaternion.identity);
    }
}
