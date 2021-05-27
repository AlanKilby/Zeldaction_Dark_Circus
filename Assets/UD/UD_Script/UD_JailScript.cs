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
        NPC_DialogueTrigger.enabled = false;
    }

    public void DestroyJail()
    {
        isBroken = true;
        anim.SetBool("Broken", isBroken);
        NPC_DialogueTrigger.enabled = true;
    }
}
