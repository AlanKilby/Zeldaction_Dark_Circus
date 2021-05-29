using System;
using UnityEngine;

public class AnimEventDestroyTarget : MonoBehaviour
{
    [SerializeField] private GameObject targetToDestroy; 
    [SerializeField, Range(0f, 2f)] private float destroyDelay = 0.25f;
    [SerializeField] private bool destroyFromStart;

    private void OnEnable()
    {
        FallDetection.OnGroundDetection += BossEventDestroy; 
    }
    
    private void OnDisable()
    {
        FallDetection.OnGroundDetection -= BossEventDestroy; 
    }

    private void Start()
    {
        if (!destroyFromStart) return; 
        Destroy(targetToDestroy, destroyDelay);  
    }

    public void DestroyTarget()
    { 
        Destroy(targetToDestroy, destroyDelay);
        // Debug.Log("destroying target from animation event"); 
    }

    private void BossEventDestroy()
    {
        Destroy(targetToDestroy, destroyDelay);
    }
}
