using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;


public class Boomerang : MonoBehaviour
{
    public float distance;
    public float speed;

    public Vector3 aimPos;
    public Transform playerPos;

    private Rigidbody rb;

    bool isGoing;
    public static bool s_IsComingBack;

    bool holder = true;

    public float comebackTimer;
    float comebackTimerHolder;

    private void Start()
    {
        comebackTimerHolder = comebackTimer;
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        isGoing = true;
        s_IsComingBack = false;
        rb = gameObject.GetComponent<Rigidbody>();
        aimPos = playerPos.GetComponent<PlayerMovement_Alan>().aim.transform.position;
        Throw();
    }

    private void Update()
    {

        playerPos = GameObject.FindGameObjectWithTag("Player").transform;

        //if (!isComingBack)
        //{
        //    rb.position = Vector3.MoveTowards(transform.position, playerPos.position, speed * Time.deltaTime);
        //}


        if (s_IsComingBack)
        {


            this.transform.LookAt(playerPos);

            
            rb.velocity = Vector3.zero;


            rb.AddForce(transform.forward * speed, ForceMode.VelocityChange);
            

            holder = false;
        }

        Teleport();
    }


    public void Throw()
    {
        StartCoroutine(Throwing());
    }
    IEnumerator Throwing()
    {
        rb.AddForce(transform.forward * speed, ForceMode.VelocityChange);

        if(s_IsComingBack == true)
        {
            yield return null;
        }
        yield return new WaitForSeconds(comebackTimer);


        s_IsComingBack = true;
        isGoing = false;
        comebackTimer = comebackTimerHolder;

        yield return null;
    }


    public void Teleport()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerPos.position = transform.position;
            s_IsComingBack = true;
            isGoing = false;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player") && s_IsComingBack == true)
        {
            Debug.Log("Collision with Player");
            playerPos.GetComponent<PlayerMovement_Alan>().canThrow = true;
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Collision with Wall");

            s_IsComingBack = true;
            isGoing = false;
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Collision with Enemy");

            Destroy(collision.gameObject);

        }
    }
}
