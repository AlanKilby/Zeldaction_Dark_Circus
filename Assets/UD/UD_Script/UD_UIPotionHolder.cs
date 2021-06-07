using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UD_UIPotionHolder : MonoBehaviour
{
    public Sprite[] potionHolderImages;
    Image ownImage;

    void Start()
    {
        ownImage = GetComponent<Image>();
    }


    void Update()
    {
        ownImage.sprite = potionHolderImages[AK_PlayerManager.potionNumber];
    }
}
