using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_DetectionZone : MonoBehaviour
{
    public bool isPlayerOn = false;
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            isPlayerOn = true;
        }
    }
}
