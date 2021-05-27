using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AK_SlowingZone : MonoBehaviour
{
    public LayerMask playerLayer;
    public LayerMask enemyLayer;

    [FormerlySerializedAs("slowSpeed")]
    [Tooltip("The speed the player will be slowed to.")]
    [Range(0f, 1f)] public float slowSpeedMultiplier = 0.5f;
    private float oldSpeed;
    private AICustomEffectOnZoneSlow _customAIEffect; 

    private void OnTriggerEnter(Collider other)
    {
        if (Mathf.Pow(2, other.gameObject.layer) == playerLayer && !other.gameObject.GetComponent<PlayerMovement_Alan>().isSlowed)
        {
            oldSpeed = other.gameObject.GetComponent<PlayerMovement_Alan>().movementSpeed;

            other.gameObject.GetComponent<PlayerMovement_Alan>().isSlowed = true;

            other.gameObject.GetComponent<PlayerMovement_Alan>().movementSpeed *= slowSpeedMultiplier;
        }
        else if (Mathf.Pow(2, other.gameObject.layer) == enemyLayer) 
        {
            _customAIEffect = other.GetComponent<AICustomEffectOnZoneSlow>(); 
            _customAIEffect.InvokeCustomEventOnEnter();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (Mathf.Pow(2, other.gameObject.layer) == playerLayer) 
        {
            other.gameObject.GetComponent<PlayerMovement_Alan>().movementSpeed = oldSpeed;

            other.gameObject.GetComponent<PlayerMovement_Alan>().isSlowed = false;
        }
        else if (Mathf.Pow(2, other.gameObject.layer) == enemyLayer)
        {
            _customAIEffect.InvokeCustomEventOnExit();  
        }
    }
}
