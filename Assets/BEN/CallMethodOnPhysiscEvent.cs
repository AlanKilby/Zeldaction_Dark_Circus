using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CallMethodOnPhysiscEvent : MonoBehaviour
{
    [SerializeField, Range(1, 5)] private int radius = 1;
    [SerializeField] private LayerMask detectableTargetsLayer;
    private BoxCollider selfCollider;
    private Collider[] detectedCollidersArray;
    private AgentPatrol patrolBehaviour;
    private bool playerDetected;
    private RaycastHit[] detectedColliders;

    private float[] distances;
    private float smallestValue;

    private bool notified; // DEBUG 

    private void Start() 
    {
        selfCollider = GetComponent<BoxCollider>();
        patrolBehaviour = GetComponentInParent<AgentPatrol>(); 
    }

    private void OnDrawGizmos() 
    {
#if UNITY_EDITOR
        /* UnityEditor.Handles.color = Color.red; 
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, radius); */ 
#endif
    }

    private void FixedUpdate()  
    {
        /* detectedCollidersArray = Physics.OverlapBox(transform.position, 
                                                    new Vector3(selfCollider.size.x, selfCollider.size.y, selfCollider.size.z), 
                                                    Quaternion.identity, 
                                                    detectableTargetsLayer);

        if (detectedCollidersArray.Length > 0 && !patrolBehaviour.playerDetected)
        {
            Debug.Log("Player is detected");
            patrolBehaviour.SetDestination(detectedCollidersArray[0].transform.position); 
        }
        else
        {


        } */ 
    }

    private void OnTriggerEnter(Collider other) 
    {
        Debug.DrawRay(transform.position, other.transform.position - transform.position, Color.red); 

        if (other.CompareTag("Player") && !patrolBehaviour.m_PlayerDetected && playerDetected)
        {
            patrolBehaviour.SetDestination(other.transform.position, 0f, playerDetected); 
        }
    }  

    private void OnTriggerStay(Collider other)
    {
        Debug.DrawRay(transform.position, other.transform.position - transform.position, Color.red);
        detectedColliders = Physics.RaycastAll(transform.position, other.transform.position - transform.position, 15f, detectableTargetsLayer);
        distances = new float[detectedColliders.Length];

        for (int i = 0; i < detectedColliders.Length; i++)
        {
            if( i == 0)
                smallestValue = detectedColliders[i].distance; 
            else if (smallestValue > detectedColliders[i].distance)
            {
                smallestValue = detectedColliders[i].distance; 
            }

            distances[i] = detectedColliders[i].distance;

            if (detectedColliders[i].transform.gameObject.CompareTag("Player"))
            {
                playerDetected = Mathf.Approximately(smallestValue, detectedColliders[i].distance); 

                if (playerDetected && !notified)
                {
                    notified = true; 
                    patrolBehaviour.SetDestination(other.transform.position, 0f, playerDetected);
                }
            }
        } 
        // blockedByWall = Physics.Raycast(transform.position, other.transform.position - transform.position, out RaycastHit hit, 15f, props);
    }

    private void OnTriggerExit(Collider other) 
    { 
        if (other.CompareTag("Player") && patrolBehaviour.m_PlayerDetected)
        {
            patrolBehaviour.SetDestination(Vector3.zero, 2f, false); // reset speed internally because you don't know initial value here 
            notified = false; 
        }
    }
}
