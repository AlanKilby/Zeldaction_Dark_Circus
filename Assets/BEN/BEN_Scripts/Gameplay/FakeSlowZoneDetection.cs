using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeSlowZoneDetection : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;
        
        Debug.Log("calling custom event");
        other.GetComponent<AICustomEffectOnZoneSlow>().InvokeCustomEvent();
    }
}
