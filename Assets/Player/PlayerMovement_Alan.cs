using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement_Alan : MonoBehaviour
{
    public float movementSpeed;

    public Rigidbody playerRB;
    
    public GameObject boomerang;

    public GameObject aim;

    public bool canThrow;
    
    // DEBUG benji
    public Vector3 move;
    public Vector3 input; 

    GameObject boomerangInstance;

    public PlayerAnimations playerAnim;

    void Start()
    {
        canThrow = true;

    }

    void Update()
    {

        input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")); 
        Move();

        if(canThrow == true)
        {
            Attack();
        }
    }

    public void Move()
    {
        float horizontalMove = Input.GetAxisRaw("Horizontal");
        float verticalMove = Input.GetAxisRaw("Vertical");
        // Vector3 move = transform.forward * verticalMove + transform.right * horizontalMove;

        move = new Vector3(horizontalMove, 0f, verticalMove).normalized;
        playerRB.velocity = move * movementSpeed;


        if (move != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(move);
            //aim.transform.position = move * 5;
        }


        // Animations
        if(horizontalMove == 0 && verticalMove == 0 && canThrow == true)
        {
            playerAnim.ChangeAnimationState(playerAnim.PLAYER_IDLE_HAT);
        }
        else if(verticalMove > 0 && canThrow == true)
        {
            playerAnim.ChangeAnimationState(playerAnim.PLAYER_TOP_HAT);
        }
        else if (verticalMove < 0 && canThrow == true)
        {
            playerAnim.ChangeAnimationState(playerAnim.PLAYER_DOWN_HAT);
        }
        else if (horizontalMove > 0 && verticalMove == 0 && canThrow == true)
        {
            playerAnim.ChangeAnimationState(playerAnim.PLAYER_RIGHT_HAT);
        }
        else if (horizontalMove < 0 && verticalMove == 0 && canThrow == true)
        {
            playerAnim.ChangeAnimationState(playerAnim.PLAYER_LEFT_HAT);
        } // NO HAT
        else if(horizontalMove == 0 && verticalMove == 0 && canThrow == false)
        {
            playerAnim.ChangeAnimationState(playerAnim.PLAYER_IDLE_NO_HAT);
        }
        else if (verticalMove > 0 && canThrow == false)
        {
            playerAnim.ChangeAnimationState(playerAnim.PLAYER_TOP_NO_HAT);
        }
        else if (verticalMove < 0 && canThrow == false)
        {
            playerAnim.ChangeAnimationState(playerAnim.PLAYER_DOWN_NO_HAT);
        }
        else if (horizontalMove > 0 && verticalMove == 0 && canThrow == false)
        {
            playerAnim.ChangeAnimationState(playerAnim.PLAYER_RIGHT_NO_HAT);
        }
        else if (horizontalMove < 0 && verticalMove == 0 && canThrow == false)
        {
            playerAnim.ChangeAnimationState(playerAnim.PLAYER_LEFT_NO_HAT);
        }



    }

    public void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            boomerangInstance = Instantiate(boomerang, gameObject.transform.position, gameObject.transform.rotation);
            canThrow = false;
        }
    }

    
}
