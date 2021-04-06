using BEN.Scripts;
using UnityEngine;
using System.Reflection;
using BEN.Scripts.FSM;
using MonsterLove.StateMachine;
using UnityEngine.AI; 

namespace BEN
{
    [RequireComponent(typeof(BoxCollider))]
    public class CheckSurroundings : MonoBehaviour
    {
        [SerializeField] private LayerMask detectableTargetsLayer;
        private BoxCollider _selfCollider;
        private Collider[] _detectedCollidersArray;
        private FsmPatrol _patrol;
        private NavMeshAgent _agent; 
        private bool _playerDetected;
        private RaycastHit[] _detectedColliders; 

        private float[] _distances; 
        private float _smallestValue;

        private bool _notified; // DEBUG 
        private BasicAIBrain _brain; // NOT SAFE

        public CheckSurroundings(Collider[] detectedCollidersArray)
        {
            _detectedCollidersArray = detectedCollidersArray;
        }

        private void Start() 
        {
            _selfCollider = GetComponent<BoxCollider>();
            // _patrol = GetComponentInParent<FsmPatrol>(); 
            _agent = GetComponentInParent<NavMeshAgent>();
            _brain = GetComponentInParent<BasicAIBrain>(); 
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
        
        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            Debug.DrawRay(transform.position, other.transform.position - transform.position, Color.red);
            // use RaycastNonAlloc instead !!
            _detectedColliders = Physics.RaycastAll(transform.position, other.transform.position - transform.position, 15f, detectableTargetsLayer);
            _distances = new float[_detectedColliders.Length];

            // check if player is not behind a wall
            for (var i = 0; i < _detectedColliders.Length; i++)
            {
                if( i == 0)
                    _smallestValue = _detectedColliders[i].distance;  
                else if (_smallestValue > _detectedColliders[i].distance)
                {
                    _smallestValue = _detectedColliders[i].distance; 
                }

                _distances[i] = _detectedColliders[i].distance;

                if (!_detectedColliders[i].transform.gameObject.CompareTag("Player")) continue;
                // player is detected if his collider is the closest to enemy
                _playerDetected = Mathf.Approximately(_smallestValue, _detectedColliders[i].distance);

                if (!_playerDetected || _notified) continue;
                
                // go to attackState 
                if (!_notified) 
                { 
                    _brain.TargetToAttackPosition = other.transform.position;
                    BasicAIBrain.OnRequireStateChange(States.Attack, StateTransition.Overwrite); 
                } 
                _notified = true; 
                // _patrol.SetDestination(other.transform.position, 0.5f, _playerDetected); if have FSMpatrol 
            } 
            
            // attack 
            // blockedByWall = Physics.Raycast(transform.position, other.transform.position - transform.position, out RaycastHit hit, 15f, props); 
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return; // || !_patrol.playerDetected) return;
            // _patrol.SetDestination(Vector3.zero, 2f, false); // reset speed internally because you don't know initial value here 
            // monkey 
            _notified = false;  
            BasicAIBrain.OnRequireStateChange(States.Default, StateTransition.Overwrite); 
        } 
    }
}
