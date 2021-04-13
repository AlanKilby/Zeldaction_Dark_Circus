using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowingZone : MonoBehaviour
{
    public float slowSpeed;
    private float oldSpeed;
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("SlowingZone"))
        {
            oldSpeed = other.gameObject.GetComponent<PlayerMovement_Alan>().movementSpeed;

            other.gameObject.GetComponent<PlayerMovement_Alan>().movementSpeed = slowSpeed;
            
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("SlowingZone"))
        {
            other.gameObject.GetComponent<PlayerMovement_Alan>().movementSpeed = oldSpeed;
        }
    }
}
