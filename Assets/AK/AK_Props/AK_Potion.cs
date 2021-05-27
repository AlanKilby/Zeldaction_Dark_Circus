using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_Potion : MonoBehaviour
{
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player")){

            collision.GetComponent<AK_PlayerPotionManager>().potionQuantity++;
            Destroy(gameObject);      
        }
    }


}
