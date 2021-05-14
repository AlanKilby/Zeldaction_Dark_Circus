using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHM_NPCTrigger : MonoBehaviour
{
    public GameObject dialogueButton;

    void Start()
    {
        dialogueButton.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            dialogueButton.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            dialogueButton.SetActive(false);
        }
    }
}
