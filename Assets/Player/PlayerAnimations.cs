﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{

    public GameObject player;
    Animator animator;
    PlayerMovement playerMovement;
    public string currentState;

    public string PLAYER_IDLE_HAT = "idleh";
    public string PLAYER_DOWN_HAT = "Walkingdownh";
    public string PLAYER_LEFT_HAT = "WalkingLefth";
    public string PLAYER_RIGHT_HAT = "Walkingrighth";
    public string PLAYER_TOP_HAT = "Walkingtoph";

    public string PLAYER_IDLE_NO_HAT = "idlewh";
    public string PLAYER_DOWN_NO_HAT = "WalkingdownNh";
    public string PLAYER_LEFT_NO_HAT = "WalkingleftNh";
    public string PLAYER_RIGHT_NO_HAT = "WalkingrightNh";
    public string PLAYER_TOP_NO_HAT = "WalkingtopNh";

    public string PLAYER_THROWING_HAT = "throwinghat";

    private void Start()
    {
        animator = GetComponent<Animator>();
        playerMovement = player.GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        transform.position = player.transform.position;
    }

    public void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;

        animator.Play(newState);

        currentState = newState;
    }



}
