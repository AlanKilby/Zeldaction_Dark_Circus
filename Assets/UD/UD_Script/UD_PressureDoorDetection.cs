using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UD_PressureDoorDetection : MonoBehaviour
{
    public AK_PressureDoor PD;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player")|| other.CompareTag("PlayerWeapon"))
        {
            PD.canClose = false;
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player")|| other.CompareTag("PlayerWeapon"))
        {
            PD.canClose = false;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player")|| other.CompareTag("PlayerWeapon"))
        {
            PD.canClose = true;
        }
    }
}
