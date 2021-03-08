using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class AgentPatrol : MonoBehaviour
{
    public Transform[] points;
    private int destPoint = 0;
    private NavMeshAgent agent;
    public bool playerDetected;
    private float distanceFromTarget;
    private bool canThrowObject = true; 

    [Header("FireSpitter")]
    public GameObject objToThrow; 

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Disabling auto-braking allows for continuous movement
        // between points (ie, the agent doesn't slow down as it
        // approaches a destination point).
        agent.autoBraking = false;

        GotoNextPoint();
    }
     
    void Update()
    { 
        // Choose the next destination point when the agent gets
        // close to the current one. 
        if (!agent.pathPending && agent.remainingDistance < 0.5f && !playerDetected)
            GotoNextPoint();

        if (playerDetected)
        {
            distanceFromTarget = Vector3.Distance(transform.position, agent.destination); 
            agent.speed = 0f;

            if (canThrowObject)
                ThrowObject();

            canThrowObject = false;
        } 
    } 

    void GotoNextPoint()
    {
        // Returns if no points have been set up
        if (points.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        agent.destination = points[destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % points.Length;
    }

    public void SetDestination(Vector3 newDestination)
    {
        agent.destination = newDestination;
        playerDetected = true;
    }


    // move this to individual behaviour 
    private void ThrowObject()
    {
        GameObject reference = Instantiate(objToThrow, transform.position, Quaternion.identity);
        reference.transform.LookAt(Camera.main.transform);

        reference.GetComponent<ParabolicFunction>().SetTargetPosition(agent.destination); 
    } 
}
