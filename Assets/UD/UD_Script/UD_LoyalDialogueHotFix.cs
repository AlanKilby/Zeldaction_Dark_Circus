using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UD_LoyalDialogueHotFix : MonoBehaviour
{
    public GameObject talkButton;

    void Start()
    {
        talkButton.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            talkButton.SetActive(true);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            talkButton.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            talkButton.SetActive(false);
        }
    }
}
