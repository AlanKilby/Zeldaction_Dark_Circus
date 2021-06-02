using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_Trampoline : MonoBehaviour
{
    Animator trampoAnimator;

    private void Start()
    {
        trampoAnimator = gameObject.GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("PlayerWeapon"))
        {
            Debug.Log("Weapon Collision");
            TrampolineBounce();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerWeapon"))
        {
            Invoke("TrampolineIdle", 1f);
        }
    }

    public void TrampolineBounce()
    {
        
        trampoAnimator.Play("tampo");
    }


    public void TrampolineIdle()
    {
        trampoAnimator.Play("Idle");
    }

}
