using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_TrapChest : MonoBehaviour
{
    Collider chestCollider;

    [Tooltip("This enemy will be spawned by the trap chest.")]
    public GameObject enemy;

    private void Awake()
    {
        chestCollider = this.GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.layer == LayerMask.NameToLayer("Hat"))
        {
            chestCollider.enabled = false;
            EnemySpawn();
        }
    }

    public void EnemySpawn()
    {
        Instantiate(enemy, this.transform.position, Quaternion.identity);
    }
}
