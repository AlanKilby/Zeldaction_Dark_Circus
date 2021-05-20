using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UD_LeverPuzzleDetectionZone : MonoBehaviour
{
    public UD_LeverPuzzle leverAssociated;
    public GameObject UI_ToShow;

    bool activationInputEnable;

    private void Start()
    {
        UI_ToShow.SetActive(false);
        activationInputEnable = false;
    }

    private void Update()
    {
        if (activationInputEnable && Input.GetKeyDown(KeyCode.Space))
        {
            leverAssociated.Activate();
        }
    }

    public void HideUI()
    {
        UI_ToShow.SetActive(false);
        activationInputEnable = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && leverAssociated.canBeActivate)
        {
            UI_ToShow.SetActive(true);
            activationInputEnable = true;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UI_ToShow.SetActive(false);
            activationInputEnable = false;
        }
    }
}
