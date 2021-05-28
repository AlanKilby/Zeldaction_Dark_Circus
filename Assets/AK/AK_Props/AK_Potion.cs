using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_Potion : MonoBehaviour
{
    Animator potionAnim;
    BoxCollider potionCollider;
    AK_PlayerPotionManager potionManager;
    public float destroyTime = 4;
    private void Start()
    {
        potionAnim = GetComponent<Animator>();
        potionCollider = GetComponent<BoxCollider>();
        potionManager = GameObject.FindGameObjectWithTag("Player").GetComponent<AK_PlayerPotionManager>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && potionManager.potionQuantity < potionManager.maxPotionQuantity)
        {

            //Debug.Log("PlayerCollisionPotion");
            potionManager.potionQuantity++;
            potionAnim.Play("collected");
            potionCollider.enabled = false;
            Destroy(gameObject,destroyTime);      
        }
    }

}
