using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_PlayerHit : MonoBehaviour
{
    public float invincibleTimer;

    public static bool isInvincible = false;


    public void CallInvincibleState()
    {
        StartCoroutine(InvincibleState());
    }

    IEnumerator InvincibleState()
    {
        if (!isInvincible)
        {
            isInvincible = true;
            yield return new WaitForSeconds(invincibleTimer);
            isInvincible = false;
        }
        else
        {
            yield return null;
        }
    }
}
