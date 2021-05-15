using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_TargetMovement : MonoBehaviour
{
    public float movementSpeed;

    public GameObject targetRight;
    public GameObject targetLeft;

    public bool movingRight;

    private void FixedUpdate()
    {
        if(transform.position != targetLeft.transform.position && !movingRight)
        {
            //transform.Translate(targetLeft.transform.position - transform.position);
            transform.Translate(-transform.right * movementSpeed * Time.deltaTime);
        }
        else if(transform.position != targetRight.transform.position && movingRight)
        {
            //transform.Translate(targetRight.transform.position - transform.position);
            transform.Translate(transform.right * movementSpeed * Time.deltaTime);
        }

        if(transform.position == targetLeft.transform.position)
        {
            movingRight = true;
        }
        else if(transform.position == targetRight.transform.position)
        {
            movingRight = false;
        }


    }
}
