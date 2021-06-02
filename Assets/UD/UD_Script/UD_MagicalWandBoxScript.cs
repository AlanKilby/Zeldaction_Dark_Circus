using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UD_MagicalWandBoxScript : MonoBehaviour
{
    Animator anim;

    public GameObject magicalWand;

    private void Start()
    {
        anim = GetComponent<Animator>();
        magicalWand.SetActive(false);
    }

    public void WandBoxOpen()
    {
        anim.Play("crate blowup");
        magicalWand.SetActive(true);
    }
}
