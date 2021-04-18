using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_SlowingZone : MonoBehaviour
{
    [Tooltip("The speed the player will be slowed to.")]
    public float slowSpeed;
    private float oldSpeed;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            oldSpeed = other.gameObject.GetComponent<PlayerMovement_Alan>().movementSpeed;

            other.gameObject.GetComponent<PlayerMovement_Alan>().movementSpeed = slowSpeed;
            
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            other.gameObject.GetComponent<PlayerMovement_Alan>().movementSpeed = oldSpeed;
        }
    }
}
