﻿using UnityEngine;

public class PlayerMovement_Alan : MonoBehaviour
{
    public float movementSpeed = 5f; 

    public Rigidbody playerRB;


    public GameObject boomerang;

    public GameObject aim;

    [HideInInspector]
    public bool canThrow;

    [Tooltip("If the player has the Wand he can teleport to his hat.")]
    public bool hasWand;

    GameObject boomerangInstance;

    public PlayerAnimations playerAnim;

    [HideInInspector]
    public bool canMove = true;

    [HideInInspector]
    public bool isInMenu = false;

    public float throwingTime;

    public float minThrowTime;

    public float maxThrowTime;

    public float chargeTimeCoeff;

    // add static reference to pos => BEN 
    public static Vector3 sPlayerPos = Vector3.zero; 
    public static GameObject sPlayer; // debug to simulate death call from AI 

    [HideInInspector]
    public bool isSlowed = false;

    void Start()
    {
        canThrow = true;
        throwingTime = minThrowTime;
        sPlayer = transform.root.gameObject; 
    }

    void Update()
    {
        if (isInMenu)
        {
            canMove = false;
            canThrow = false;
        }
        else if (!isInMenu)
        {
            canMove = true;
            canThrow = true;
        }
        
        sPlayerPos = transform.position; 

        if (canThrow == true)
        {
            if (Input.GetButton("PlayerAttack"))
            {
                canMove = false;
                if(throwingTime <= maxThrowTime)
                {
                    throwingTime += Time.deltaTime * chargeTimeCoeff;
                }
            }
            if (Input.GetButtonUp("PlayerAttack"))
            {
                Debug.Log("No Longer Pressing");
                Attack(throwingTime);
                canMove = true;
                throwingTime = minThrowTime;
            }
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void Move()
    {
        float horizontalMove = Input.GetAxisRaw("Horizontal");
        float verticalMove = Input.GetAxisRaw("Vertical");


        // Movement direction (x,y,z)
        Vector3 move = new Vector3(horizontalMove, 0f, verticalMove).normalized;


        // Lock player movement if !canMove
        if(canMove == true)
        {
            //playerRB.MovePosition(transform.position + move * movementSpeed * Time.deltaTime);
            playerRB.velocity = new Vector3(move.x * movementSpeed,0f, move.z * movementSpeed);
        }
        
        // Keep current rotation
        if (move != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(move);
        }


        // Animations 
        if (Input.GetButton("PlayerAttack") && canThrow)
        {
            if(horizontalMove > 0 && verticalMove == 0 && canThrow)
            {
                playerAnim.ChangeAnimationState(playerAnim.PLAYER_THROWING_HAT_RIGHT);
            }
            else if(horizontalMove < 0 && verticalMove == 0 && canThrow)
            {
                playerAnim.ChangeAnimationState(playerAnim.PLAYER_THROWING_HAT_LEFT);
            }
            else if(verticalMove > 0 && canThrow)
            {
                playerAnim.ChangeAnimationState(playerAnim.PLAYER_THROWING_HAT_TOP);
            }
            else if(verticalMove < 0 && canThrow)
            {
                playerAnim.ChangeAnimationState(playerAnim.PLAYER_THROWING_HAT_DOWN);
            }
        }
        else
        {
            if (horizontalMove == 0 && verticalMove == 0 && canThrow == true)
            {
                playerAnim.ChangeAnimationState(playerAnim.PLAYER_IDLE_HAT);
            }
            else if (verticalMove < 0 && canThrow == true)
            {
                playerAnim.ChangeAnimationState(playerAnim.PLAYER_DOWN_HAT);
            }
            else if (verticalMove > 0 && canThrow == true)
            {
                playerAnim.ChangeAnimationState(playerAnim.PLAYER_TOP_HAT);
            }
            else if (horizontalMove > 0 && verticalMove == 0 && canThrow == true)
            {
                playerAnim.ChangeAnimationState(playerAnim.PLAYER_RIGHT_HAT);
            }
            else if (horizontalMove < 0 && verticalMove == 0 && canThrow == true)
            {
                playerAnim.ChangeAnimationState(playerAnim.PLAYER_LEFT_HAT);
            } // NO HAT
            else if (horizontalMove == 0 && verticalMove == 0 && canThrow == false)
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
        
    } 

    public void Attack(float throwTime)
    {
        boomerangInstance = Instantiate(boomerang, aim.transform.position, gameObject.transform.rotation);
        boomerangInstance.GetComponent<Boomerang>().comebackTimer = throwTime;
        canThrow = false;
    }
}
