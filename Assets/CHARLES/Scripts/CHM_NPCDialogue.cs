using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHM_NPCDialogue : MonoBehaviour
{
    //Le dialogue lié à ce bouton particulier (lui-même lié à un NPC)
    public CHM_Dialogue dialogue;

    //Ajout Ulric
    public CHM_DialogueManager ownDialogueManager;
    private PlayNPCSound _npcSound;

    private void Start()
    {
        _npcSound = GetComponent<PlayNPCSound>();
    }

    public void TriggerDialogue()
    {
        ownDialogueManager.StartDialogue(dialogue);
        _npcSound.PlaySound(); 
    }

}