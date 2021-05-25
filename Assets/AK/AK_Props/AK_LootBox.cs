using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_LootBox : MonoBehaviour
{
    Collider chestCollider;
    MeshRenderer cubeMesh;

    [HideInInspector]
    public AK_DropRateManager dropRateManager;
    private void Awake()
    {
        chestCollider = GetComponent<Collider>();
        cubeMesh = GetComponent<MeshRenderer>();
        dropRateManager = GameObject.FindGameObjectWithTag("Player").GetComponent<AK_DropRateManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.layer == LayerMask.NameToLayer("PlayerWeapon"))
        {
            chestCollider.enabled = false;
            cubeMesh.enabled = false;
            dropRateManager.ObjectDrop();
        }
    }

    
}
