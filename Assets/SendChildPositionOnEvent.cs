using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BEN.Scripts.FSM; 

public class SendChildPositionOnEvent : MonoBehaviour
{
    private void OnEnable()
    {
        BasicAIBrain.OnQueryingChildPosition += SendChildPosition; 
    }

    private void OnDisable()
    {
        BasicAIBrain.OnQueryingChildPosition -= SendChildPosition;  
    }

    private Transform[] SendChildPosition()
    {
        Transform[] childPositionArray = new Transform[transform.childCount]; 
        for (var i = 0; i < transform.childCount; i++)
        {
            childPositionArray[i] = transform.GetChild(i).transform; 
        }

        return childPositionArray; 
    }
}
