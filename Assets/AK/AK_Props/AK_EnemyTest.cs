using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AK_EnemyTest : MonoBehaviour
{
    public UnityEvent playerHit;


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("EventCalled");
        playerHit.Invoke();
    }
}
