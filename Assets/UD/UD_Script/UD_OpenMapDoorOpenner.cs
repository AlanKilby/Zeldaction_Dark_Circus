using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UD_OpenMapDoorOpenner : MonoBehaviour
{
    public Animator animLeft;
    public Animator animRight;

    private void Start()
    {
        animLeft.SetBool("isOpen", false);
        animRight.SetBool("isOpen", false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animLeft.SetBool("isOpen", true);
            animRight.SetBool("isOpen", true);
        }
    }
}
