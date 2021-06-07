using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CHM_DialogueButton : MonoBehaviour
{
    public UnityEvent FunctionCalled;

    void Update()
    {
        if (Input.GetButtonDown("PlayerInteraction"))
        {

            FunctionCalled.Invoke();
        }
    }
}
