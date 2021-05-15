using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 1 event = 1 scriptable object
// avoid having events all over the place without knowing who calls and who subscribes 
public class GameEventListener : MonoBehaviour
{
    public GameEventSO Event;
    public UnityEvent Response; 

    private void OnEnable() { Event.RegisterListener(this); }
    private void OnDisable() { Event.UnregisterListener(this); }
    public void OnEventRaised() { Response.Invoke(); }  
    // need to specify the UnityEvent(s) with functions 

    public void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.K)) 
            Event.Raise();
    } 
}
