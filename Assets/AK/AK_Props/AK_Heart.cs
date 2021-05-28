using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_Heart : MonoBehaviour
{
    public float heartValue = 1;
    Health playerHealth;
    AgentGameplayData playerData;
    private void Start()
    {

        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && playerHealth.CurrentValue < playerData.Value)
        {
            playerHealth.CurrentValue += (sbyte)heartValue;
            Destroy(gameObject);
        }
    }
}
