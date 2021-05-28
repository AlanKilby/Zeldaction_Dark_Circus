using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_Heart : MonoBehaviour
{
    public float heartValue = 1;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            playerHealth.CurrentValue += (sbyte)heartValue;
            Destroy(gameObject);
        }
    }
}
