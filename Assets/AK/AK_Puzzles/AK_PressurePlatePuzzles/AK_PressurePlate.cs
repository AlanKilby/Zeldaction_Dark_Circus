using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_PressurePlate : MonoBehaviour
{
    public float pressureTimer;
    private float timerHolder;

    public bool isPressing;

    public Renderer ownPlateFeeback;
    public Material OnMat;
    public Material OffMat;

    private void Awake()
    {
        timerHolder = pressureTimer;
    }

    private void Start()
    {
        pressureTimer = 0;
        ownPlateFeeback.material = OffMat;
    }

    private void Update()
    {
        if(pressureTimer > 0)
        {
            isPressing = true;
            pressureTimer -= Time.deltaTime;
            ownPlateFeeback.material = OnMat;
        }
        else if (pressureTimer <= 0)
        {
            isPressing = false;
            pressureTimer = 0;
            ownPlateFeeback.material = OffMat;
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
