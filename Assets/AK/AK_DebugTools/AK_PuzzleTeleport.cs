using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_PuzzleTeleport : MonoBehaviour
{
    public GameObject positionPuzzle1;

    public GameObject positionPuzzle2;

    public GameObject positionPuzzle3;

    public bool toggleDebug = false;

    public GameObject dungeonDoor;

    PlayerMovement_Alan playerMovement;
    private void Start()
    {
        playerMovement = gameObject.GetComponent<PlayerMovement_Alan>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1) && !toggleDebug) // Debug ON
        {
            Debug.Log("Debug mode : ON");
            toggleDebug = true;
        }
        else if(Input.GetKeyDown(KeyCode.F1) && toggleDebug) // Debug OFF
        {
            Debug.Log("Debug mode : OFF");
            toggleDebug = false;
        }


        if (toggleDebug)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) // Teleport to Puzzle 1
            {
                transform.position = positionPuzzle1.transform.position;
            }

            if (Input.GetKeyDown(KeyCode.Alpha2)) // Teleport to Puzzle 2
            {
                transform.position = positionPuzzle2.transform.position;
            }

            if (Input.GetKeyDown(KeyCode.Alpha3)) // Teleport to Puzzle 3
            {
                transform.position = positionPuzzle3.transform.position;
            }

            if (Input.GetKeyDown(KeyCode.C)) // Get Back Hat
            {
                playerMovement.canThrow = true;
            }

            if (Input.GetKeyDown(KeyCode.P)) // Get 1 potion
            {
                AK_PlayerManager.potionNumber++;
            }

            if (Input.GetKeyDown(KeyCode.I) && !AK_PlayerHit.isInvincible) // Invincible ON
            {
                AK_PlayerHit.isInvincible = true;
            }
            else if(Input.GetKeyDown(KeyCode.I) && AK_PlayerHit.isInvincible) // Invincible OFF
            {
                AK_PlayerHit.isInvincible = false;
            }

            if (Input.GetKeyDown(KeyCode.O)) // Open Dungeon door
            {
                dungeonDoor.SetActive(true);
            }
        }
    }
}
