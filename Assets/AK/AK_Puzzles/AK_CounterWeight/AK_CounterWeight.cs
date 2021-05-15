using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class AK_CounterWeight : MonoBehaviour
{
    public GameObject detectionZone;
    private Rigidbody cwRb;
    public GameObject otherCounterWeight;
    private Rigidbody otherCwRb;
    private Vector3 startingPos;

    public GameObject highPos;
    public GameObject lowPos;

    private void Start()
    {
        startingPos = transform.position;
        cwRb = GetComponent<Rigidbody>();
        otherCwRb = otherCounterWeight.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (detectionZone.GetComponent<AK_DetectionZone>().isPlayerOn)
        {
            cwRb.MovePosition(lowPos.transform.position);
            otherCwRb.MovePosition(highPos.transform.position);
        }
        else if (!detectionZone.GetComponent<AK_DetectionZone>().isPlayerOn)
        {
            cwRb.MovePosition(startingPos);
            otherCwRb.MovePosition(highPos.transform.position);
        }
    }
}
