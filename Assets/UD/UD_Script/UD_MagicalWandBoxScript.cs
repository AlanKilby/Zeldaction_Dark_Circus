using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UD_MagicalWandBoxScript : MonoBehaviour
{
    Animator anim;

    public GameObject magicalWand;
    Collider ownCollider;

    private void Start()
    {
        anim = GetComponent<Animator>();
        magicalWand.SetActive(false);
        ownCollider = GetComponent<Collider>();
        ownCollider.enabled = true;
    }

    public void WandBoxOpen()
    {
        anim.Play("crate blowup");
        magicalWand.SetActive(true);
        ownCollider.enabled = false;
    }
}
