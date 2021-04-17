using UnityEngine;
using BEN.Scripts.FSM;
using BEN; 

public class Boomerang : MonoBehaviour
{
    public float distance;
    public float speed;

    public Vector3 aimPos;
    public Transform playerPos;

    private Rigidbody rb;

    bool isGoing;
    bool isComingBack;

    bool holder = true;

    public float comebackTimer;
    float comebackTimerHolder;

    private AIType enemyType; 

    private void Start()
    {
        comebackTimerHolder = comebackTimer;
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        isGoing = true;
        isComingBack = false;
        rb = gameObject.GetComponent<Rigidbody>();
        aimPos = playerPos.GetComponent<PlayerMovement_Alan>().aim.transform.position;
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
            rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime); // Test for the hat movement
            comebackTimer -= Time.deltaTime;
        }
        else if(comebackTimer <= 0)
        {
            isComingBack = true;
            isGoing = false;
        }


        if (isComingBack)
        {


            this.transform.LookAt(playerPos);

            
            rb.velocity = Vector3.zero;


            //rb.AddForce(transform.forward * speed, ForceMode.VelocityChange);

            rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);

            holder = false;
        }

        Teleport();
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
            isGoing = false;
            playerPos.GetComponent<PlayerMovement_Alan>().canThrow = true;
            Destroy(gameObject);
            
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
            isGoing = false;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Debug.Log("Collision with Enemy");
            enemyType = other.GetComponent<BasicAIBrain>().Type; 
            float value = Vector3.Dot(other.transform.localPosition.normalized, transform.localPosition.normalized); 

            if (enemyType == AIType.Mascotte && value > 0f) 
            {
                Destroy(other.gameObject); 
            } 
            else if (enemyType != AIType.Mascotte)
            { 
                Destroy(other.gameObject);
            }
        }
    }
}
