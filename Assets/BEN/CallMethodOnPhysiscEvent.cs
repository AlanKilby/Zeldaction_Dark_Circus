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
        detectedCollidersArray = Physics.OverlapBox(transform.position, 
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


        }
    }
}
