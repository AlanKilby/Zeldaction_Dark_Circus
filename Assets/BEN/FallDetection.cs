using System;
using UnityEngine;
using UnityEngine.Events;

public class FallDetection : MonoBehaviour
{
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private UnityEvent _OnGroundDetection;
    [SerializeField] private GameObject _destructionEffect;
    public static Action OnGroundDetection; 

    private void OnTriggerEnter(Collider other)
    {
        if (Mathf.Pow(2f, other.gameObject.layer) == _groundLayer)
        {
            _OnGroundDetection.Invoke(); // play sound and destroy shadow + self
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (Mathf.Pow(2f, other.gameObject.layer) == _groundLayer)
        { 
            _OnGroundDetection.Invoke(); // play sound and destroy shadow + self
            OnGroundDetection(); 
        } 
    }

    private void OnDestroy()
    {
        if (!Application.isPlaying) return; 
        Instantiate(_destructionEffect, transform.position, Quaternion.identity);
    }
}
