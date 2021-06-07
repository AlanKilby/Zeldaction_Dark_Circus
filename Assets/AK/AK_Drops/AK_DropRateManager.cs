using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_DropRateManager : MonoBehaviour
{
    [Header("Drops general information")]
    [SerializeField] private DifficultySettings _difficultySettings;
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
        minDropRate = _difficultySettings.Value switch
        {
            Difficulty.Easy => minDropRate * 1.75f,
            Difficulty.Hard => minDropRate * 0.7f,
            _ => minDropRate
        }; 

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
            currentDropRate = maxDropRate;
        

        if (currentDropRate < minDropRate)
            currentDropRate = minDropRate;
    }
    
    public void Drop(GameObject thisGameObject)
    {
        if(Random.Range(0,101) <= currentDropRate)
        {
            currentDropRate = minDropRate;
            ObjectDrop(thisGameObject);
        }
        else 
        {
            IncrementDropRate();
        }
    } 

    private void ObjectDrop(GameObject thisGameObject)
    {
        float randomPicker = Random.Range(0, 101);

        if(randomPicker <= potionDropRate)
        {
            Instantiate(potion, thisGameObject.transform.position, Quaternion.identity);
        }
        else if(randomPicker > potionDropRate)
        {
            Instantiate(heart,thisGameObject.transform.position,Quaternion.identity);
        }
    }
    private void IncrementDropRate()
    {
        currentDropRate += dropRateIncrease;
    }
}
