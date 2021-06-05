using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UD_JailScript : MonoBehaviour
{
    public Collider NPC_DialogueTrigger;

    Animator anim;

    bool isBroken;

    void Start()
    {
        anim = GetComponent<Animator>();
        isBroken = false;
        if(NPC_DialogueTrigger != null)
        {
            NPC_DialogueTrigger.enabled = false;
        }
    }

    public void DestroyJail()
    {
        isBroken = true;
        anim.SetBool("Broken", isBroken);
        if (NPC_DialogueTrigger != null)
        {
            NPC_DialogueTrigger.enabled = true;
        }
    }
}
