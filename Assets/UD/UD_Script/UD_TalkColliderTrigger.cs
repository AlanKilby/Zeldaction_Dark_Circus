using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UD_TalkColliderTrigger : MonoBehaviour
{
    bool dialogueFinished;
    CHM_NPCDialogue ownDialogue;

    private void Start()
    {
        ownDialogue = GetComponent<CHM_NPCDialogue>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ownDialogue.TriggerDialogue();
        }
    }
}
