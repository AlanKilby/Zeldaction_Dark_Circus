using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_HatJumpPad : MonoBehaviour
{
    public LayerMask playerWeapon;

    [Tooltip("Extra time added to the hat flight time.")]
    public float extraTime;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == playerWeapon)
        {
            other.GetComponent<Boomerang>().comebackTimer += extraTime;
        }
    }
}
