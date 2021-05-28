using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UD_LeverPuzzleManager : MonoBehaviour
{
    public UD_LeverPuzzleDoor doorToOpen;

    public UD_LeverPuzzle[] leverList;

    [SerializeField] private float timeBetweenLeverSpoting;
    [HideInInspector] public float timeBetweenLeverSpotingTimer;

    [HideInInspector] public int currentActivatedLevers;
    int counter;

    bool puzzleStopped;

    void Start()
    {
        RefreshLeverListID();
        timeBetweenLeverSpotingTimer = 0;
        puzzleStopped = false;
    }


    void Update()
    {
        if (currentActivatedLevers >= leverList.Length && !puzzleStopped)
        {
            doorToOpen.OpenDoors();
            puzzleStopped = true;
            for (int i = 0; i < leverList.Length; i++)
            {
                leverList[i].DiseableActivation();
            }
        }

        if (timeBetweenLeverSpotingTimer > 0 && !puzzleStopped)
        {
            timeBetweenLeverSpotingTimer -= Time.deltaTime;
        }

        if (timeBetweenLeverSpotingTimer <= 0 && !puzzleStopped)
        {
            if (counter < leverList.Length)
            {
                if(currentActivatedLevers < leverList.Length-1)
                {
                    leverList[counter].DiseableActivation();
                    print("desactivated");
                }
                counter++;
            }
            if(counter >= leverList.Length)
            {
                counter = 0;
            }

            if (leverList[counter].isActivated)
            {
                while (leverList[counter].isActivated && currentActivatedLevers < leverList.Length)
                {
                    counter++;
                    if (counter >= leverList.Length)
                    {
                        counter = 0;
                    }
                }
            }
            else
            {
            }
            leverList[counter].canBeActivate = true;
            timeBetweenLeverSpotingTimer = timeBetweenLeverSpoting;
        }
    }

    public void RefreshLeverListID()
    {
        for(int i = 0; i < leverList.Length; i++)
        {
            leverList[i].ownID = i;
        }
    }

}
