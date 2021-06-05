using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_TargetMovement : MonoBehaviour
{
    public float movementSpeed;

    public GameObject targetRight;
    public GameObject targetLeft;

    public bool movingRight;

    AK_DuckScript DS;
    SpriteRenderer SR;

    private void Start()
    {
        DS = GetComponent<AK_DuckScript>();
        SR = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if(transform.position != targetLeft.transform.position && !movingRight && !DS.wasShot)
        {
            //transform.Translate(targetLeft.transform.position - transform.position);
            transform.Translate(-transform.right * movementSpeed * Time.deltaTime);
        }
        else if(transform.position != targetRight.transform.position && movingRight && !DS.wasShot)
        {
            //transform.Translate(targetRight.transform.position - transform.position);
            transform.Translate(transform.right * movementSpeed * Time.deltaTime);
        }

        if(transform.position.x <= targetLeft.transform.position.x)
        {
            movingRight = true;
        }
        else if(transform.position.x >= targetRight.transform.position.x)
        {
            movingRight = false;            
        }

        if (movingRight)
        {
            SR.flipX = false;
        }
        else { SR.flipX = true; }


    }
}
