using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UD_BossHealthBar : MonoBehaviour
{
    float maxHP;

    public Health bossHealth;
    Image image;

    void Start()
    {
        maxHP = bossHealth.CurrentValue;
        image = GetComponent<Image>();
    }

    void Update()
    {
        image.fillAmount = bossHealth.CurrentValue / maxHP;
    }
}
