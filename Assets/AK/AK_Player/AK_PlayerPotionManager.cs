using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_PlayerPotionManager : MonoBehaviour
{

    public int potionQuantity;

    public int maxPotionQuantity;

    public int potionHealthValue;

    string potion = "potion";

    Health playerHealth;

    float maxHP;

    private void Start()
    {
        playerHealth = GetComponent<Health>();
        maxHP = playerHealth.CurrentValue;
    }   
    private void Update()
    {
        

        if (Input.GetButtonDown("Potion") && AK_PlayerManager.potionNumber > 0 && playerHealth.CurrentValue < maxHP)
        {
            UsePotion(potion);
        }
       
        else if (Input.GetButtonDown("Potion") && AK_PlayerManager.potionNumber <= 0)
        {
            Debug.Log("You've got no potions left.");
        }
        

    }

    public void UsePotion(string potionType)
    {
        AK_PlayerManager.potionNumber--;
        //potionQuantity--;
        playerHealth.CurrentValue += (sbyte)potionHealthValue;
        
        
    }
}
