using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace BEN.Scripts.FSM
{
    public class FsmPatrol : MonoBehaviour
    {
        public Transform[] points;
        private int _destPoint = 0;
        private NavMeshAgent _agent;
        [FormerlySerializedAs("_playerDetected")] public bool playerDetected;
        private float _distanceFromTarget;

        private void Start()
        {
            _agent = GetComponent<NavMeshAgent>();

            // Disabling auto-braking allows for continuous movement
            // between points (ie, the agent doesn't slow down as it
            // approaches a destination point).
            _agent.autoBraking = false;

            GotoNextPoint(); 
        }

        private void FixedUpdate() 
        { 
            // Choose the next destination point when the agent gets
            // close to the current one. 
            if (!_agent.pathPending && _agent.remainingDistance < 0.5f && !playerDetected)
                GotoNextPoint();


            if (!playerDetected) return;
            _distanceFromTarget = Vector3.Distance(transform.position, _agent.destination); 
            _agent.speed = 0f;
        } 

        void GotoNextPoint() 
        {
            // Returns if no points have been set up
            if (points.Length == 0)
                return;

            // Set the agent to go to the currently selected destination.
            _agent.destination = points[_destPoint].position;

            // Choose the next point in the array as the destination,
            // cycling to the start if necessary.
            _destPoint = (_destPoint + 1) % points.Length;
        }

        public void SetDestination(Vector3 newDestination, float speed, bool plDetected)
        {
            _agent.destination = newDestination;
            playerDetected = plDetected;
            _agent.speed = speed; 
        }

        public void Stop()
        {
            _agent.speed = 0f; 
        }
    }
}
