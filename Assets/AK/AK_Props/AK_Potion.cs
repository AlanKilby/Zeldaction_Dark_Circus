using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_Potion : MonoBehaviour
{
    Animator potionAnim;
    BoxCollider potionCollider;

    public float destroyTime = 4;
    private void Start()
    {
        potionAnim = GetComponent<Animator>();
        potionCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player")){

            Debug.Log("PlayerCollisionPotion");
            collision.GetComponent<AK_PlayerPotionManager>().potionQuantity++;
            potionAnim.Play("collected");
            potionCollider.enabled = false;
            Destroy(gameObject,destroyTime);      
        }
    }

}
