using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{

    public GameObject player;
    Animator animator;
    PlayerMovement_Alan playerMovement;
    public string currentState;

    bool isThrowing = false;

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

    public string PLAYER_THROWING_HAT_TOP = "hat charge top";
    public string PLAYER_THROWING_HAT_DOWN = "hat charge bot";
    public string PLAYER_THROWING_HAT_RIGHT = "hat charge right";
    public string PLAYER_THROWING_HAT_LEFT = "hat charge left";
    public string PLAYER_THROW_ANIM = "throwinghat";
    public string PLAYER_DEAD = "death";

    public string PLAYER_HIT_HAT_TOP = "hittop";
    public string PLAYER_HIT_HAT_DOWN = "hitbot";
    public string PLAYER_HIT_HAT_RIGHT = "hitright";
    public string PLAYER_HIT_HAT_LEFT = "hitleft";

    public string PLAYER_HIT_NO_HAT_TOP = "hittopnh";
    public string PLAYER_HIT_NO_HAT_DOWN = "hitbotnh";
    public string PLAYER_HIT_NO_HAT_RIGHT = "hitrightnh";
    public string PLAYER_HIT_NO_HAT_LEFT = "hitleftnh";

    [Space, SerializeField] private Health _playerHp;

    //Ajout Ulric
    public UD_FadeAndLoadingSceneManager Fade;
    public int deathScreenIndex;
    //

    private void Start()
    {
        animator = GetComponent<Animator>();
        playerMovement = player.GetComponent<PlayerMovement_Alan>();
        _playerHp.IsMonkeyBall = false; 
    }

    private void FixedUpdate()
    {
        transform.position = player.transform.position;

        if (_playerHp.CurrentValue <= 0 && !animator.GetCurrentAnimatorStateInfo(0).IsName("death")) 
        {
            ChangeAnimationState(PLAYER_DEAD);
            playerMovement.canMove = false;
            playerMovement.canThrow = false;
            Debug.Log("player death animation"); 
        }
    }

    public void ChangeAnimationState(string newState)
    {
        if (currentState == newState || animator.GetCurrentAnimatorStateInfo(0).IsName("death") || isThrowing) return;

        animator.Play(newState);
        currentState = newState;
    } 


    public void LoadDeathScreen()
    {
        Fade.nextSceneIndex = deathScreenIndex;
        Fade.ChangeAnimationState(Fade.Loyal_Fade_In);
    }

    public void ThrowingAnimBoolToTrue()
    {
        isThrowing = true;
    }

    public void ThrowingAnimBoolToFalse()
    {
    
        isThrowing = false;

    }
}
