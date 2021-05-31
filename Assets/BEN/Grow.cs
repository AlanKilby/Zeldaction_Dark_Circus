using System;
using UnityEngine;

public class Grow : MonoBehaviour
{
    [SerializeField, Range(0.2f, 3f)] private float _growthSpeed = 0.75f;
    [SerializeField] public bool isSpotlightShadow;

    private void OnEnable()
    {
        FallDetection.OnGroundDetection += DestroyShadowOnSpotlightCall; 
    }

    private void OnDisable()
    {
        FallDetection.OnGroundDetection -= DestroyShadowOnSpotlightCall; 
    }
 
    void Start() 
    {
        if (!isSpotlightShadow) enabled = false; 
        
        _growthSpeed *= 0.02f;
        transform.localScale = new Vector3(0.2f, 0.2f, 0.2f); 
    }

    void FixedUpdate() 
    {
        transform.localScale = new Vector3(transform.localScale.x + _growthSpeed, transform.localScale.y + _growthSpeed, transform.localScale.z); 
    }

    private void DestroyShadowOnSpotlightCall()
    {
        Destroy(gameObject); 
    }
} 
