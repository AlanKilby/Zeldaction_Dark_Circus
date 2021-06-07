using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UD_BossLeverUI : MonoBehaviour
{
    public GameObject YCanvas;
    Switch ownSwitch;

    bool playerIsNear;

    void Start()
    {
        playerIsNear = false;
        YCanvas.SetActive(false);
        ownSwitch = GetComponent<Switch>();
    }

    void Update()
    {
        if (playerIsNear)
        {
            YCanvas.SetActive(true);
        }
        else
        {
            YCanvas.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && ownSwitch.CanBeDeactivated)
        {
            playerIsNear = true;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && ownSwitch.CanBeDeactivated)
        {
            playerIsNear = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && ownSwitch.CanBeDeactivated)
        {
            playerIsNear = false;
        }
    }
}
