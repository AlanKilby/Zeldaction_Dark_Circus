using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_Potion : MonoBehaviour
{
    public bool isSmallPotion;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player")){

            if (isSmallPotion)
            {
                collision.GetComponent<AK_PlayerPotionManager>().smallPotionQuantity++;
                Destroy(gameObject);
            }
            else if (!isSmallPotion)
            {
                collision.GetComponent<AK_PlayerPotionManager>().bigPotionQuantity++;
                Destroy(gameObject);
            }
        }
    }


}
