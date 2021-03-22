using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace BEN.Scripts
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class AgentPatrol : MonoBehaviour
    {
        public Transform[] points;
        private int _destPoint = 0;
        private NavMeshAgent _agent;
        [FormerlySerializedAs("m_PlayerDetected")] public bool playerDetected;
        private float _distanceFromTarget;
        private bool _canThrowObject = true; 
        public bool isMascotte = false;  // to be removed

        [Header("FireSpitter")]
        public GameObject objToThrow; 
        
        private bool canCAC = true; 

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
            _agent.speed = 0f + Utility.BoolToInt(isMascotte);

            if (_canThrowObject && !isMascotte)
                ThrowObject();
            else if (isMascotte && _distanceFromTarget <= 2f)
            {
                CacAttack(); 
            }

            _canThrowObject = false;
        }

        private void CacAttack()
        {
            // DEBUG 
            Gizmos.color = Color.yellow; 
            Gizmos.DrawWireSphere(transform.position, 5f); 
            StartCoroutine(SetAttackBool()); 
        }

        private void GotoNextPoint() 
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

        // move this to individual behaviour 
        private void ThrowObject()
        {
            var reference = Instantiate(objToThrow, transform.position, Quaternion.identity);
            reference.transform.LookAt(Camera.main.transform);

            reference.GetComponent<ParabolicFunction>().SetTargetPosition(_agent.destination, transform.position);

            StartCoroutine(SetBool()); 
        } 

        private IEnumerator SetBool() 
        {
            yield return new WaitForSeconds(5f);
            _canThrowObject = true; 
        }

        private IEnumerator SetAttackBool()
        {
            canCAC = false; 
            yield return new WaitForSeconds(3f);
            canCAC = true;   
        }
    }
}
