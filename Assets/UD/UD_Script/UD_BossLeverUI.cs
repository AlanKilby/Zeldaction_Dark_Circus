using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UD_BossLeverUI : MonoBehaviour
{
    public GameObject YCanvas; 
    Switch ownSwitch;

    public bool PlayerIsNear { get; set; }

    void Start()
    {
        PlayerIsNear = false;
        YCanvas.SetActive(false);
        ownSwitch = GetComponent<Switch>();
    }

    void Update()
    {
        if (PlayerIsNear)
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
            PlayerIsNear = true;
        }
    } 

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && ownSwitch.CanBeDeactivated)
        {
            PlayerIsNear = false;
        }
    }
} 
