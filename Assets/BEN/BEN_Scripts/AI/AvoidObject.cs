using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI; 

public class AvoidObject : MonoBehaviour
{
    public static Action<Vector3> OnAvoiding;
    private bool canJump = true;
    public NavMeshAgent ballAgent;
    private float initialSpeed;
    private bool newStateSet;

    public Behaviour[] behavioursToActivateOnStateChange;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canJump)
            Avoid(); 
        else if (Input.GetKeyDown(KeyCode.X) && !newStateSet) // ball destroyed and monkey attacking
        {
            newStateSet = true; 
            SetNewState(); 
        } 
    }

    void Avoid()
    {
        // simulating jump
        initialSpeed = ballAgent.speed; 
        ballAgent.speed = 0f;  
        transform.position = new Vector3(transform.position.x + UnityEngine.Random.Range(-1, 2),
                                         transform.position.y,
                                         transform.position.z + UnityEngine.Random.Range(-1, 2));

        // for the ball
        StartCoroutine(nameof(SendNewPosition)); 
    } 

    IEnumerator SendNewPosition()
    {
        yield return new WaitForSeconds(0.05f);
        OnAvoiding(transform.position);
        canJump = false; 

        yield return new WaitForSeconds(2f);
        ballAgent.speed = initialSpeed;

        yield return new WaitForSeconds(3f); 
        canJump = true;
    }

    void SetNewState()
    {
        Transform reference = gameObject.transform.parent;
        reference.DetachChildren();
        Destroy(reference.gameObject, 0.2f); 

        transform.position = new Vector3(transform.position.x, 0.3f, transform.position.z);
        Invoke(nameof(ActivateComponents), 0.5f); 
    } 

    private void ActivateComponents()
    {
        for (int i = 0; i < behavioursToActivateOnStateChange.Length; i++)
        {
            behavioursToActivateOnStateChange[i].enabled = true;
        }
    }

    private void ResetLocalPosition()
    {
        float initialLocalHeight = transform.localPosition.y;
        transform.localPosition = new Vector3(0f, initialLocalHeight, 0f); 
    }
}
