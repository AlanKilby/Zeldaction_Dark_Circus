using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Grow : MonoBehaviour
{
    [SerializeField, Range(0.2f, 3f)] private float _growthSpeed = 0.75f;
    [SerializeField] public bool isSpotlightShadow;
    public int Index { get; set; }

    private void OnEnable()
    {
        FallDetection.OnGroundDetection += DestroyShadowOnSpotlightCall;
        FallDetection.NotifyShadowOnReachingGround += StopGrowOnNotify;
    }

    private void OnDisable()
    {
        FallDetection.OnGroundDetection -= DestroyShadowOnSpotlightCall; 
        FallDetection.NotifyShadowOnReachingGround -= StopGrowOnNotify;
    }
 
    void Start() 
    {
        if (!isSpotlightShadow)
        {
            enabled = false;
            return; 
        } 
        
        _growthSpeed *= 0.02f;
        transform.localScale = new Vector3(0.2f, 0.2f, 0.2f); 
    }

    void FixedUpdate()
    {
        transform.localScale = new Vector3(transform.localScale.x + _growthSpeed, transform.localScale.y + _growthSpeed, transform.localScale.z); 
    }

    private void DestroyShadowOnSpotlightCall()
    {
        Debug.Log("destroying shadow"); 
        Destroy(gameObject); 
    }

    private void StopGrowOnNotify(int index)
    {
        if (index != Index) return;
        enabled = false;
    }
} 
