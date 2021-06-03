using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AK_SlowingZone : MonoBehaviour
{
    public LayerMask playerLayer;
    public LayerMask enemyLayer;
    public LayerMask ballLayer;

    [FormerlySerializedAs("slowSpeed")]
    [Tooltip("The speed the player will be slowed to.")]
    [Range(0f, 1f)] public float slowSpeedMultiplier = 0.5f;
    private float oldSpeed;
    private AICustomEffectOnZoneSlow _customAIEffect;

    
    public ParticleSystem handsParticles;

    private void OnTriggerEnter(Collider other)
    {
        if (Mathf.Pow(2, other.gameObject.layer) == playerLayer && !other.gameObject.GetComponent<PlayerMovement_Alan>().isSlowed)
        {
            //oldSpeed = other.gameObject.GetComponent<PlayerMovement_Alan>().movementSpeed;

            other.gameObject.GetComponent<PlayerMovement_Alan>().isSlowed = true;

            other.gameObject.GetComponent<PlayerMovement_Alan>().movementSpeed = other.gameObject.GetComponent<PlayerMovement_Alan>().movementSpeedHolder * slowSpeedMultiplier;

            handsParticles.Play();
        }
        else if (Mathf.Pow(2, other.gameObject.layer) == enemyLayer || Mathf.Pow(2, other.gameObject.layer) ==ballLayer) 
        {
            _customAIEffect = other.GetComponent<AICustomEffectOnZoneSlow>(); 
            _customAIEffect.InvokeOnEnter(); 

            handsParticles.Play();
        } 
    }

    private void OnTriggerExit(Collider other)
    {
        if (Mathf.Pow(2, other.gameObject.layer) == playerLayer) 
        {
            other.gameObject.GetComponent<PlayerMovement_Alan>().movementSpeed = other.gameObject.GetComponent<PlayerMovement_Alan>().movementSpeedHolder;

            other.gameObject.GetComponent<PlayerMovement_Alan>().isSlowed = false;

            handsParticles.Stop();
        }
        else if (Mathf.Pow(2, other.gameObject.layer) == enemyLayer || Mathf.Pow(2, other.gameObject.layer) ==ballLayer)
        {
            _customAIEffect.InvokeOnExit();

            handsParticles.Stop();
        }
    }
}
