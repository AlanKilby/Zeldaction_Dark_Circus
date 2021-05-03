using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_PressureDoor : MonoBehaviour
{
    private BoxCollider doorCollider;
    private MeshRenderer doorMeshRenderer;

    [Tooltip("This door requires 1 or 2 plates pressed to be opened.")]
    public bool two_plates;

    [Tooltip("Pressure plates.")]
    public GameObject[] pressurePlates;

    private void Start()
    {
        doorCollider = GetComponent<BoxCollider>();
        doorMeshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        
        if(two_plates && pressurePlates[0].GetComponent<AK_PressurePlate>().isPressing && pressurePlates[1].GetComponent<AK_PressurePlate>().isPressing)
        {
            doorCollider.enabled = false;
            doorMeshRenderer.enabled = false;
        }
        else if (!two_plates && pressurePlates[0].GetComponent<AK_PressurePlate>().isPressing)
        {
            doorCollider.enabled = false;
            doorMeshRenderer.enabled = false;
        }
        else
        {
            doorCollider.enabled = true;
            doorMeshRenderer.enabled = true;
        }
    }
}
