using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

public class AvoidObject : MonoBehaviour
{
    public static Action<Vector3> OnAvoiding;
    private bool canJump = true;
    public MoveTo ballMovement; 

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canJump)
            Avoid(); 
        /* else if (Input.GetKeyDown(KeyCode.X)) // ball destroyed and monkey attacking
        {
            SetNewState(); 
        } */
    }

    void Avoid()
    {
        // simulating jump
        ballMovement.enabled = false; 
        transform.position = new Vector3(transform.position.x + UnityEngine.Random.Range(-1, 1),
                                         transform.position.y,
                                         transform.position.z + UnityEngine.Random.Range(-1, 1));

        // for the ball
        StartCoroutine(nameof(SendNewPosition)); 
    } 

    void SetNewState()
    {
        Transform reference = gameObject.transform.parent;
        reference.DetachChildren();
        Destroy(reference, 0.2f); 
    }

    IEnumerator SendNewPosition()
    {
        yield return new WaitForSeconds(0.05f);
        OnAvoiding(transform.position);
        canJump = false;

        yield return new WaitForSeconds(2f);
        ballMovement.enabled = canJump = true;
    }
}
