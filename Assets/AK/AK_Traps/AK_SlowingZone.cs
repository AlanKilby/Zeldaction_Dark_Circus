using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AK_SlowingZone : MonoBehaviour
{
    public LayerMask playerLayer;

    [FormerlySerializedAs("slowSpeed")]
    [Tooltip("The speed the player will be slowed to.")]
    [Range(0f, 1f)] public float slowSpeedMultiplier = 0.5f;
    private float oldSpeed;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == playerLayer)
        {
            oldSpeed = other.gameObject.GetComponent<PlayerMovement_Alan>().movementSpeed;

            other.gameObject.GetComponent<PlayerMovement_Alan>().movementSpeed *= slowSpeedMultiplier;
        } 
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == playerLayer)
        {
            other.gameObject.GetComponent<PlayerMovement_Alan>().movementSpeed = oldSpeed;
        }
    }
}
