using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UD_UIPotionHolder : MonoBehaviour
{
    public Sprite[] potionHolderImages;
    Image ownImage;

    AK_PlayerPotionManager PPM;

    void Start()
    {
        PPM = GameObject.FindGameObjectWithTag("Player").GetComponent<AK_PlayerPotionManager>();
        ownImage = GetComponent<Image>();
    }


    void Update()
    {
        ownImage.sprite = potionHolderImages[PPM.potionQuantity];
    }
}
