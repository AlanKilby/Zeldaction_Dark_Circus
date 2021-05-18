using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_DropRateManager : MonoBehaviour
{
    [Header("Drops general information")]
    [Tooltip("The player's drop rate will never go below this number.")]
    [Range(0f,100f)]
    public float minDropRate;

    float currentDropRate;

    float maxDropRate = 100;

    [Tooltip("The amount the drop rate will increase at each event.")]
    public float dropRateIncrease;

    [Header("Over time increase")]
    [Tooltip("Will the drop rate increase over time?")]
    public bool increaseOT = false;

    [Tooltip("Seconds between each increase over time.")]
    public float time;
    float timeHolder;

    [Header("Instantiable objects")]
    [Tooltip("Potion to instantiate.")]
    public GameObject potion;
    public float potionDropRate;

    [Tooltip("Heart to instantiate.")]
    public GameObject heart;
    [SerializeField]
    private float heartDropRate;


    private void Start()
    {
        heartDropRate = 100 - potionDropRate;
        timeHolder = time;
        currentDropRate = minDropRate;
    }

    private void Update()
    {
        if (increaseOT)
        {
            if(timeHolder > 0)
            {
                timeHolder -= Time.deltaTime;
            }
            else if(timeHolder <= 0)
            {
                IncrementDropRate();
                timeHolder = time;
            }
        }

        if(currentDropRate >= maxDropRate)
        {
            Drop();
            currentDropRate = minDropRate;
        }

        if (currentDropRate < minDropRate)
            currentDropRate = minDropRate;
    }

    public void Drop()
    {
        if(Random.Range(0,101) <= currentDropRate)
        {
            currentDropRate = minDropRate;
            PotionDrop();
        }
        else
        {
            IncrementDropRate();
        }
    }

    public void PotionDrop()
    {
        float randomPicker = Random.Range(0, 101);

        if(randomPicker <= potionDropRate)
        {
            Instantiate(potion);
        }
        else if(randomPicker > potionDropRate)
        {
            Instantiate(heart);
        }
    }
    public void IncrementDropRate()
    {
        currentDropRate += dropRateIncrease;
    }
}
