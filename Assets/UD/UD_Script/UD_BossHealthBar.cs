using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(15)]

public class UD_BossHealthBar : MonoBehaviour
{
    float maxHP;

    Health bossHealth;
    Image image;
    private float value;

    void Start()
    {
        image = GetComponent<Image>();
        bossHealth = GameObject.FindGameObjectWithTag("Boss").GetComponent<Health>();
        maxHP = bossHealth.CurrentValue; 
    }

    void Update()
    {
        value = (bossHealth.CurrentValue / maxHP);
        image.fillAmount = value;
    }
}
