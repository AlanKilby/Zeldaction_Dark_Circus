using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_PlayerHit : MonoBehaviour
{
    public float invincibleTimer;

    CapsuleCollider playerCollider;

    public SpriteRenderer playerSpriteRenderer;

    bool isInvincible = false;

    public bool callFunction;

    private void Start()
    {
        playerCollider = gameObject.GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        if (callFunction)
        {
            CallInvincibleState();
        }
    }
    public void CallInvincibleState()
    {
        StartCoroutine(InvincibleState());
    }

    IEnumerator InvincibleState()
    {
        if (!isInvincible)
        {
            isInvincible = true;
            playerCollider.enabled = false;
            yield return new WaitForSeconds(invincibleTimer);

            isInvincible = false;
            playerCollider.enabled = true;


        }
        else
        {
            yield return null;
        }
    }
}
