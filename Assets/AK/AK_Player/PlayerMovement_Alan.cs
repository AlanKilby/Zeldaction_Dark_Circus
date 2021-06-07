using System.Collections;
using UnityEngine;

public class PlayerMovement_Alan : MonoBehaviour
{
    [HideInInspector]
    public float horizontalMove;
    [HideInInspector]
    public float verticalMove;

    public float movementSpeed = 5f;

    [HideInInspector]
    public float movementSpeedHolder;

    public Rigidbody playerRB;


    public GameObject boomerang;

    public GameObject aim;

    //[HideInInspector]
    public bool canThrow;

    [Tooltip("If the player has the Wand he can teleport to his hat.")]
    public bool hasWand;

    GameObject boomerangInstance;

    public PlayerAnimations playerAnim;

    //[HideInInspector]
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

    AK_PlayerHit playerHit;

    public bool isHit = false;

    public SpriteRenderer aimSpriteRend;

    public Animator hitScreen;

    //Ajout Ulric
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] hitSounds;
    [SerializeField] private UD_ScreenShakeForEnnemies SS;
    //

    void Start()
    {
        canThrow = true;
        throwingTime = minThrowTime;
        sPlayer = transform.root.gameObject;
        movementSpeedHolder = movementSpeed;
        playerHit = gameObject.GetComponent<AK_PlayerHit>();
        aimSpriteRend.enabled = false;

    }

    void Update()
    {

        PlayerAnims();
        PlayerNormalAnims();
        
        sPlayerPos = transform.position; 

        if (canThrow == true)
        {
            if (Input.GetButton("PlayerAttack"))
            {
                playerRB.velocity = Vector3.zero;
                canMove = false;
                aimSpriteRend.enabled = true;
                if(throwingTime <= maxThrowTime)
                {
                    throwingTime += Time.deltaTime * chargeTimeCoeff;
                }
            }
            if (Input.GetButtonUp("PlayerAttack"))
            {
                // Debug.Log("No Longer Pressing");
                Attack(throwingTime);
                canMove = true;
                throwingTime = minThrowTime;
                aimSpriteRend.enabled = false;
            }
        }

        if (!isSlowed)
        {
            movementSpeed = movementSpeedHolder;
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void Move()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");
        verticalMove = Input.GetAxisRaw("Vertical");


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

    } 

    

    public void Attack(float throwTime)
    {
        boomerangInstance = Instantiate(boomerang, aim.transform.position, gameObject.transform.rotation);
        boomerangInstance.GetComponent<Boomerang>().comebackTimer = throwTime;
        canThrow = false;
    }

    public void HitAnim()
    {
        StartCoroutine(HitAnimation());
    }
    IEnumerator HitAnimation()
    {
        int i = Random.Range(0, hitSounds.Length);

        audioSource.PlayOneShot(hitSounds[i]);

        if (SS != null)
        {
            SS.MediumScreenShake();
        }

        isHit = true;

        hitScreen.Play("hit");

        yield return new WaitForSeconds(0.5f);

        isHit = false;
    }
    public void PlayerNormalAnims()
    {
        // Animations 
        if (Input.GetButton("PlayerAttack") && canThrow)
        {
            if (horizontalMove > 0 && verticalMove == 0 && canThrow)
            {
                playerAnim.ChangeAnimationState(playerAnim.PLAYER_THROWING_HAT_RIGHT);
            }
            else if (horizontalMove < 0 && verticalMove == 0 && canThrow)
            {
                playerAnim.ChangeAnimationState(playerAnim.PLAYER_THROWING_HAT_LEFT);
            }
            else if (verticalMove > 0 && canThrow)
            {
                playerAnim.ChangeAnimationState(playerAnim.PLAYER_THROWING_HAT_TOP);
            }
            else if (verticalMove < 0 && canThrow)
            {
                playerAnim.ChangeAnimationState(playerAnim.PLAYER_THROWING_HAT_DOWN);
            }
        }
        else if (Input.GetButtonUp("PlayerAttack") && canThrow)
        {
            playerAnim.ChangeAnimationState(playerAnim.PLAYER_THROW_ANIM);
        }
        else if(!isHit && canMove)
        {
            // NO HIT
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
    public void PlayerAnims()
    {
        if (isHit)
        {
            // HITS
            if (horizontalMove == 0 && verticalMove == 0 && canThrow == true)
            {
                playerAnim.ChangeAnimationState(playerAnim.PLAYER_HIT_HAT_IDLE);
            }
            else if (verticalMove < 0 && canThrow == true)
            {
                playerAnim.ChangeAnimationState(playerAnim.PLAYER_HIT_HAT_DOWN);
                Debug.Log("playerAnim.PLAYER_HIT_HAT_DOWN");
            }
            else if (verticalMove > 0 && canThrow == true)
            {
                playerAnim.ChangeAnimationState(playerAnim.PLAYER_HIT_HAT_TOP);
                Debug.Log("playerAnim.PLAYER_HIT_HAT_TOP");
            }
            else if (horizontalMove > 0 && verticalMove == 0 && canThrow == true)
            {
                playerAnim.ChangeAnimationState(playerAnim.PLAYER_HIT_HAT_RIGHT);
                Debug.Log("playerAnim.PLAYER_HIT_HAT_RIGHT");
            }
            else if (horizontalMove < 0 && verticalMove == 0 && canThrow == true)
            {
                playerAnim.ChangeAnimationState(playerAnim.PLAYER_HIT_HAT_LEFT);
                Debug.Log("playerAnim.PLAYER_HIT_HAT_LEFT");
            } // NO HAT
            else if (horizontalMove == 0 && verticalMove == 0 && canThrow == false)
            {
                playerAnim.ChangeAnimationState(playerAnim.PLAYER_HIT_NO_HAT_IDLE);
            }
            else if (verticalMove > 0 && canThrow == false)
            {
                playerAnim.ChangeAnimationState(playerAnim.PLAYER_HIT_NO_HAT_TOP);
                Debug.Log("playerAnim.PLAYER_HIT_NO_HAT_TOP");
            }
            else if (verticalMove < 0 && canThrow == false)
            {
                playerAnim.ChangeAnimationState(playerAnim.PLAYER_HIT_NO_HAT_DOWN);
                Debug.Log("playerAnim.PLAYER_HIT_NO_HAT_DOWN");
            }
            else if (horizontalMove > 0 && verticalMove == 0 && canThrow == false)
            {
                playerAnim.ChangeAnimationState(playerAnim.PLAYER_HIT_NO_HAT_RIGHT);
                Debug.Log("playerAnim.PLAYER_HIT_NO_HAT_RIGHT");
            }
            else if (horizontalMove < 0 && verticalMove == 0 && canThrow == false)
            {
                playerAnim.ChangeAnimationState(playerAnim.PLAYER_HIT_NO_HAT_LEFT);
                Debug.Log("playerAnim.PLAYER_HIT_NO_HAT_LEFT");
            }
        }
    }
}
