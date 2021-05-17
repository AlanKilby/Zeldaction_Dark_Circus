using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_Wand : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("Player has wand.");
            other.GetComponent<PlayerMovement_Alan>().hasWand = true;
            Destroy(gameObject);
        }
    }
}
