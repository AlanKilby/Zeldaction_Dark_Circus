using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHM_NPCDialogue : MonoBehaviour
{
    //Le dialogue li� � ce bouton particulier (lui-m�me li� � un NPC)
    public CHM_Dialogue dialogue;

    public void TriggerDialogue()
    {
        FindObjectOfType<CHM_DialogueManager>().StartDialogue(dialogue);


    }

}