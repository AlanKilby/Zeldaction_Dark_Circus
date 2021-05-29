using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_PressureDoor : MonoBehaviour
{
    private BoxCollider doorCollider;

    [Tooltip("This door requires 1 or 2 plates pressed to be opened.")]
    public bool two_plates;

    [Tooltip("Pressure plates.")]
    public GameObject[] pressurePlates;

    //private MeshRenderer doorMeshRenderer; //MIS DE COTE CAR ON PASSE MTN PAR L'ANIMATOR - ULRIC
    public Animator animDoor;
    public string currentState;

    public string PressureDoor_Open = "PressureDoorOpen";
    public string PressureDoor_Close = "PressureDoorClose";
    
    [HideInInspector] public bool canClose;

    private void Start()
    {
        doorCollider = GetComponent<BoxCollider>();
        //doorMeshRenderer = GetComponent<MeshRenderer>();
        canClose = true;
    }

    private void Update()
    {
        
        if(two_plates && pressurePlates[0].GetComponent<AK_PressurePlate>().isPressing && pressurePlates[1].GetComponent<AK_PressurePlate>().isPressing)
        {
            doorCollider.enabled = false;
            //doorMeshRenderer.enabled = false;
            ChangeAnimationState(PressureDoor_Open);
        }
        else if (!two_plates && pressurePlates[0].GetComponent<AK_PressurePlate>().isPressing)
        {
            doorCollider.enabled = false;
            //doorMeshRenderer.enabled = false;
            ChangeAnimationState(PressureDoor_Open);
        }
        else if(canClose)
        {
            doorCollider.enabled = true;
            //doorMeshRenderer.enabled = true;
            ChangeAnimationState(PressureDoor_Close);
        }
    }

    public void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;

        animDoor.Play(newState);
        currentState = newState;
    }
}
