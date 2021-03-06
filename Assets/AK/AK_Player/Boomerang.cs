﻿using UnityEngine;
using BEN.AI;
using BEN.Math;
using MonsterLove.StateMachine;

public class Boomerang : MonoBehaviour
{
    [HideInInspector]
    public float speed;

    [Range(1, 10)] public sbyte boomerangDamage = 1;



    public Vector3 aimPos;
    public Transform playerPos;

    public AnimationCurve goingSpeedC;
    public AnimationCurve comingSpeedC;

    public GameObject teleportParticles;
    public GameObject hitParticles;

    public LayerMask mirrorLayer, playerLayer, wallLayer, enemyLayer, fakirWeaponLayer, bossLayer, jailLayer, weakPointLayer; 
   
    private Rigidbody rb;

    [HideInInspector]
    public bool isStunned;

    bool isComingBack;

    bool holder = true;

    bool hasWand;

    public float comebackTimer;
    float comebackTimerHolder;

    private BasicAIBrain enemy;
    public static bool s_SeenByEnemy;
    private AnimEventPlaySound _animEventPlaySound; 

    private void Start()
    {
        comebackTimerHolder = comebackTimer;
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        hasWand = playerPos.GetComponent<PlayerMovement_Alan>().hasWand;
        isComingBack = false;
        rb = gameObject.GetComponent<Rigidbody>();
        aimPos = playerPos.GetComponent<PlayerMovement_Alan>().aim.transform.position;
        _animEventPlaySound = GetComponent<AnimEventPlaySound>();

        s_SeenByEnemy = false; 
    }

    private void Update()
    {

        playerPos = GameObject.FindGameObjectWithTag("Player").transform;

        comebackTimer -= Time.deltaTime;


        Teleport();
        Bounce();
    }

    private void FixedUpdate()
    {
        if (!isStunned)

        {

            if (comebackTimer > 0)

            {

                //.MovePosition(transform.position + transform.forward * goingSpeed * Time.deltaTime); // Test for the hat movement



                rb.MovePosition(transform.position + transform.forward * goingSpeedC.Evaluate(comebackTimer) * Time.deltaTime);

            }

            else if (comebackTimer <= 0)

            {

                isComingBack = true;



                this.transform.LookAt(playerPos);





                rb.velocity = Vector3.zero;



                // Debug.Log(comingSpeedC.Evaluate(comebackTimer));



                rb.MovePosition(transform.position + transform.forward * comingSpeedC.Evaluate(comebackTimer) * Time.deltaTime);



                //comingSpeed = comingSpeedC.Evaluate(comebackTimer);



                holder = false;

            }

        }  
    }

    public void Teleport()
    {
        if (Input.GetButtonUp("PlayerTeleport") && hasWand)
        {
            playerPos.position = new Vector3(transform.position.x, playerPos.position.y, transform.position.z);
            isComingBack = true;
            playerPos.GetComponent<PlayerMovement_Alan>().canThrow = true;
            Instantiate(teleportParticles, gameObject.transform.position, Quaternion.identity);
            Destroy(gameObject);
            
        }
    }

    public void Bounce()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 0.5f, mirrorLayer))
        {
            Vector3 reflectDir = Vector3.Reflect(ray.direction, hit.normal);
            float rot = 90 - Mathf.Atan2(reflectDir.z, reflectDir.x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0, rot, 0);
            //comebackTimer++;
        }
    }
 

    private void OnTriggerEnter(Collider other) 
    {
        _animEventPlaySound.PlaySoundOverwrite(SoundType.Attack);

        if (Mathf.Pow(2, other.gameObject.layer) == playerLayer) 
        {
            // Debug.Log("Collision with Player");
            playerPos.GetComponent<PlayerMovement_Alan>().canThrow = true;
            playerPos.GetComponent<PlayerMovement_Alan>().playerRB.velocity = Vector3.zero;
            Destroy(gameObject);
        }

        if (Mathf.Pow(2, other.gameObject.layer) == wallLayer)
        {
            Debug.Log("Collision with Wall");

            isComingBack = true; 
            comebackTimer = 0;
        }

        if (Mathf.Pow(2, other.gameObject.layer) == weakPointLayer)
        {
            Debug.Log("hitting weak point");
            isComingBack = true; 
            comebackTimer = 0;
            other.GetComponent<Health>().DecreaseHp(boomerangDamage);
            return; 
        } 
        else if (Mathf.Pow(2, other.gameObject.layer) == enemyLayer) 
        { 
            // Debug.Log("Collision with Enemy");
            enemy = other.GetComponent<BasicAIBrain>();
            if (enemy.Type == AIType.Mascotte)  
            {
                isComingBack = true; 
                comebackTimer = 0;
                enemy.OnRequireStateChange(States.Defend, StateTransition.Safe); 
                return;
            }  

            other.GetComponent<Health>().DecreaseHp(boomerangDamage); 

            Instantiate(hitParticles, gameObject.transform.position, Quaternion.identity);

            // Changement pour que la nervosité augmente, changement fait le 19 mai 2021

            isComingBack = true;
            comebackTimer = 0;
        }

        if (Mathf.Pow(2, other.gameObject.layer) == fakirWeaponLayer) // fakir weapon
        {
            isComingBack = true; 
            comebackTimer = 0;
            other.GetComponent<ParabolicFunction>().InvertDirection(); 
        } 
        
        if (Mathf.Pow(2, other.gameObject.layer) == bossLayer) // fakir weapon
        {
            BossAIBrain.sHitCounter++; 
            // Debug.Log("hitting boss"); 
            other.GetComponent<Health>().DecreaseHp(boomerangDamage); // unefficient get component
            Instantiate(hitParticles, gameObject.transform.position, Quaternion.identity);
            isComingBack = true;
            comebackTimer = 0;
        } 
        
        if (Mathf.Pow(2, other.gameObject.layer) == jailLayer)
        {
            isComingBack = true;
            comebackTimer = 0; 
            other.GetComponent<UD_JailScript>().DestroyJail(); 
        } 
    }
} 
