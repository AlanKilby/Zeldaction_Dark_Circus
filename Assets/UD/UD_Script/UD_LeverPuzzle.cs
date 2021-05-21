using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UD_LeverPuzzle : MonoBehaviour
{
    public GameObject ownSpotLight;
    public GameObject detectionZone;
    public UD_LeverPuzzleManager manager;

    Animator anim;

    [HideInInspector] public int ownID;

    [HideInInspector] public bool canBeActivate;
    [HideInInspector] public bool isActivated;

    void Start()
    {
        isActivated = false;
        //anim.SetBool("Activated", false);
        //anim = GetComponent<Animator>();
    }
    
    void Update()
    {
        if (canBeActivate)
        {
            ownSpotLight.SetActive(true);
            detectionZone.SetActive(true);
        }
        else
        {
            ownSpotLight.SetActive(false);
            detectionZone.SetActive(false);
            DiseableActivation();
        }
    }

    public void Activate()
    {
        //anim.SetBool("Activated", true);
        manager.timeBetweenLeverSpotingTimer = 0;
        manager.currentActivatedLevers++;
        canBeActivate = false;
        isActivated = true;
    }

    public void DiseableActivation()
    {
        canBeActivate = false;
        detectionZone.GetComponent<UD_LeverPuzzleDetectionZone>().HideUI();
    }
}
