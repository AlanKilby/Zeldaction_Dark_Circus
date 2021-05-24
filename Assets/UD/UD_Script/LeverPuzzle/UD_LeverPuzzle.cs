using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UD_LeverPuzzle : MonoBehaviour
{
    public GameObject ownSpotLight;
    public GameObject detectionZone;
    public UD_LeverPuzzleManager manager;

    [HideInInspector] public int ownID;

    [HideInInspector] public bool canBeActivate;
    [HideInInspector] public bool isActivated;

    Animator anim;
    private string currentAnimation;
    const string LeverOff = "off";
    const string LeverOn = "on";

    void Start()
    {
        isActivated = false;
        anim = GetComponent<Animator>();
        ChangeAnimationState(LeverOff);
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
        ChangeAnimationState(LeverOn);
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

    void ChangeAnimationState(string newAnimation)
    {
        if (currentAnimation == newAnimation) return;

        anim.Play(newAnimation);

        currentAnimation = newAnimation;
    }
}
