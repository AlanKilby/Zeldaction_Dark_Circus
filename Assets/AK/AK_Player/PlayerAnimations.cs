using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{

    public GameObject player;
    Animator animator;
    PlayerMovement_Alan playerMovement;
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
    public string PLAYER_DEAD = "playerFakeDeath";  

    [Space, SerializeField] private Health _playerHp;

    private void Start()
    {
        animator = GetComponent<Animator>();
        playerMovement = player.GetComponent<PlayerMovement_Alan>();
    }

    private void FixedUpdate()
    {
        transform.position = player.transform.position;

        if (_playerHp.CurrentValue <= 0 && !animator.GetCurrentAnimatorStateInfo(0).IsName("playerFakeDeath"))
        {
            ChangeAnimationState(PLAYER_DEAD); 
            Debug.Log("player death animation"); 
        }
    }

    public void ChangeAnimationState(string newState)
    {
        if (currentState == newState || animator.GetCurrentAnimatorStateInfo(0).IsName("playerFakeDeath")) return;

        animator.Play(newState);
        currentState = newState;
    } 
}
