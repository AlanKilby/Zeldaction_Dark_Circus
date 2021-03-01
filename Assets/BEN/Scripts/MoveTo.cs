using UnityEngine;
using UnityEngine.AI;

public class MoveTo : MonoBehaviour
{
    public Transform goal;

    void Start()
    {
        // make this called by AI when player is in range, and override speed with custom one (preset stored in a ScriptableObject)
        NavMeshAgent agent = GetComponent<NavMeshAgent>(); 
        agent.destination = goal.position; 
    }
}

