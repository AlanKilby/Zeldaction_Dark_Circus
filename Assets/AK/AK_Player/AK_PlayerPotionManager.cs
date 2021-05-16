using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_PlayerPotionManager : MonoBehaviour
{

    public int bigPotionQuantity;
    public int smallPotionQuantity;

    public int smallPotionHealthValue;
    public int bigPotionHealthValue;

    string smallPotion = "smallPotion";
    string bigPotion = "bigPotion";

    Health playerHealth;

    private void Start()
    {
        playerHealth = GetComponent<Health>();
    }
    private void Update()
    {

        if (Input.GetButtonDown("SmallPotion") && smallPotionQuantity > 0)
        {
            UsePotion(smallPotion);
        }
        else if (Input.GetButtonDown("BigPotion") && smallPotionQuantity > 0)
        {
            UsePotion(smallPotion);
        }
        else if (Input.GetButtonDown("SmallPotion") && smallPotionQuantity <= 0)
        {
            Debug.Log("You've got no small potions left.");
        }
        else if (Input.GetButtonDown("BigPotion") && bigPotionQuantity <= 0)
        {
            Debug.Log("You've got no big potions left.");
        }

    }

    public void UsePotion(string potionType)
    {
        if(potionType == smallPotion)
        {
            smallPotionQuantity--;
            playerHealth.CurrentValue += (sbyte)smallPotionHealthValue;
        }
        else if(potionType == bigPotion)
        {
            bigPotionQuantity--;
            playerHealth.CurrentValue += (sbyte)bigPotionHealthValue;
        }
    }
}
