using UnityEngine;
using BEN.Scripts.FSM;
using BEN.Scripts; 
using BEN;
using System;

public class Boomerang : MonoBehaviour
{
   
    public float speed;
    float goingSpeed;
    float comingSpeed;

    public Vector3 aimPos;
    public Transform playerPos;

    public LayerMask mirrorMask;

    private Rigidbody rb;

    bool isComingBack;

    bool holder = true;

    [Tooltip("Percentage of speed reduction after throw.")]
    [Range(0f,1f)]
    public float reductionCoef;

    [Tooltip("Percentage of speed augmentation when Hat is coming back.")]
    [Range(0f, 1f)]
    public float accelerationCoef;

    public float comebackTimer;
    float comebackTimerHolder;

    private AIType enemyType;
    public LayerMask swordLayer; 

    private void Start()
    {
        comebackTimerHolder = comebackTimer;
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        isComingBack = false;
        rb = gameObject.GetComponent<Rigidbody>();
        aimPos = playerPos.GetComponent<PlayerMovement_Alan>().aim.transform.position;
        goingSpeed = speed;
        comingSpeed = speed;
        //rb.AddForce(transform.forward * speed, ForceMode.VelocityChange);
        //Throw();
    }

    private void Update()
    {

        playerPos = GameObject.FindGameObjectWithTag("Player").transform;

        //========================= WIP =============================================
        //if (!isComingBack)
        //{
        //    rb.position = Vector3.MoveTowards(transform.position, playerPos.position, speed * Time.deltaTime);
        //}


        if (comebackTimer > 0)
        {
            rb.MovePosition(transform.position + transform.forward * goingSpeed * Time.deltaTime); // Test for the hat movement

            // Speed reduction Limit
            if(goingSpeed > speed * 0.75)
            {
                goingSpeed -= (goingSpeed * (1-reductionCoef)) * Time.deltaTime;
            }
            comebackTimer -= Time.deltaTime;
        }
        else if(comebackTimer <= 0)
        {
            isComingBack = true;
        }


        if (isComingBack)
        {


            this.transform.LookAt(playerPos);

            
            rb.velocity = Vector3.zero;


            //rb.AddForce(transform.forward * speed, ForceMode.VelocityChange);

            rb.MovePosition(transform.position + transform.forward * comingSpeed * Time.deltaTime);

            comingSpeed += (goingSpeed * (1 + accelerationCoef)) * Time.deltaTime;

            holder = false;
        }

        Teleport();
        Bounce();
    }

    // This method and coroutine are no longer used, all is done in the Update =================================================
    //public void Throw()
    //{
    //    StartCoroutine(Throwing());
    //}
    //IEnumerator Throwing()
    //{
    //    rb.AddForce(transform.forward * speed, ForceMode.VelocityChange);

    //    if(isComingBack == true)
    //    {
    //        yield return null;
    //    }
    //    yield return new WaitForSeconds(comebackTimer);


    //    isComingBack = true;
    //    isGoing = false;
    //    comebackTimer = comebackTimerHolder;

    //    yield return null;
    //}
    //==========================================================================================================================

    public void Teleport()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            playerPos.position = new Vector3(transform.position.x, playerPos.position.y, transform.position.z);
            isComingBack = true;
            playerPos.GetComponent<PlayerMovement_Alan>().canThrow = true;
            Destroy(gameObject);
            
        }
    }

    public void Bounce()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 0.5f, mirrorMask))
        {
            Vector3 reflectDir = Vector3.Reflect(ray.direction, hit.normal);
            float rot = 90 - Mathf.Atan2(reflectDir.z, reflectDir.x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0, rot, 0);
            comebackTimer++;
        }
    }
    // ========================= WIP =============================================
    //private void OnTriggerEnter(Collider collision)
    //{
    //    if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && isComingBack == true)
    //    {
    //        Debug.Log("Collision with Player");
    //        playerPos.GetComponent<PlayerMovement>().canThrow = true;
    //        Destroy(gameObject);
    //    }

    //    if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
    //    {
    //        Debug.Log("Collision with Wall");

    //        isComingBack = true;
    //        isGoing = false;
    //    }

    //    if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
    //    {
    //        Debug.Log("Collision with Enemy");
    //        Destroy(collision.gameObject);

    //    }


    //}

    private void OnTriggerEnter(Collider other)
    {
        /*  ========================= WIP =============================================
        if (collision.gameObject.layer == LayerMask.NameToLayer("Mirror"))
        {
            Debug.Log("Collision with Mirror");

            Vector3 wallNormal = collision.contacts[0].normal;
            Vector3 newDirection = Vector3.Reflect(rb.velocity, wallNormal);

            transform.rotation = Quaternion.Euler(newDirection);

            rb.AddForce(transform.forward * speed, ForceMode.VelocityChange);
        }
        */

        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) 
        {
            Debug.Log("Collision with Player");
            playerPos.GetComponent<PlayerMovement_Alan>().canThrow = true;
            playerPos.GetComponent<PlayerMovement_Alan>().playerRB.velocity = Vector3.zero;
            Destroy(gameObject);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Debug.Log("Collision with Wall");

            isComingBack = true;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy")) 
        { 
            Debug.Log("Collision with Enemy");
            enemyType = other.GetComponent<BasicAIBrain>().Type; 
            float value = Vector3.Dot(other.transform.localPosition.normalized, transform.localPosition.normalized);

            if (enemyType == AIType.Mascotte && value < 0f)
            {
                isComingBack = true;
                return;
            }
            Destroy(other.gameObject);        
        }

        if (other.CompareTag("EnemyWeapon")) // fakir weapon
        {
            other.GetComponent<ParabolicFunction>().InvertDirection(); 
        }
    }
}
