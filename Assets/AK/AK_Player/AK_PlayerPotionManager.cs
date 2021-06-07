using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_PlayerPotionManager : MonoBehaviour
{

    public int potionQuantity;

    public int maxPotionQuantity;

    public int potionHealthValue;

    Health playerHealth;

    float maxHP;

    private void Start()
    {
        playerHealth = GetComponent<Health>();
        maxHP = 5;
    }   
    private void Update()
    {
        potionQuantity = AK_PlayerManager.potionNumber;

        if (Input.GetButtonDown("Potion") && AK_PlayerManager.potionNumber > 0 && playerHealth.CurrentValue < maxHP)
        {
            UsePotion();
        }
       
        else if (Input.GetButtonDown("Potion") && AK_PlayerManager.potionNumber <= 0)
        {
            Debug.Log("You've got no potions left.");
        }
        

    }

    public void UsePotion()
    {
        Debug.Log("Used Potion");
        AK_PlayerManager.potionNumber--;
        //potionQuantity--;
        playerHealth.CurrentValue += (sbyte)potionHealthValue;
        
        
    }
}
