using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_PressurePlate : MonoBehaviour
{
    public float pressureTimer;
    private float timerHolder;

    public bool isPressing;

    private void Awake()
    {
        timerHolder = pressureTimer;
    }

    private void Start()
    {
        pressureTimer = 0;
    }

    private void Update()
    {
        if(pressureTimer > 0)
        {
            isPressing = true;
            pressureTimer -= Time.deltaTime;
        }
        else if (pressureTimer <= 0)
        {
            isPressing = false;
            pressureTimer = 0;
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            pressureTimer = timerHolder;
        }
    }

   
}
