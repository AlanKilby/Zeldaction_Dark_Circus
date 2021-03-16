using BEN.Scripts;
using UnityEngine;
using System.Reflection;
using BEN.Scripts.FSM;

namespace BEN
{
    [RequireComponent(typeof(BoxCollider))]
    public class CheckSurroundings : MonoBehaviour
    {
        [SerializeField] private LayerMask detectableTargetsLayer;
        private BoxCollider _selfCollider;
        private Collider[] _detectedCollidersArray;
        private FsmPatrol _patrol;
        private bool _playerDetected;
        private RaycastHit[] _detectedColliders; 

        private float[] _distances; 
        private float _smallestValue;

        private bool _notified; // DEBUG 

        public CheckSurroundings(Collider[] detectedCollidersArray)
        {
            _detectedCollidersArray = detectedCollidersArray;
        }

        private void Start() 
        {
            _selfCollider = GetComponent<BoxCollider>();
            _patrol = GetComponentInParent<FsmPatrol>(); 
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

        /* private void OnTriggerEnter(Collider other) 
        {
            Debug.DrawRay(transform.position, other.transform.position - transform.position, Color.red);
            
            if (other.CompareTag("Player") && !_patrol.playerDetected && _playerDetected)
            {
                _patrol.SetDestination(other.transform.position, 0f, _playerDetected); 
            }  
        } */
        
        
        private void OnTriggerStay(Collider other)
        {
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
                _notified = true; 
                
                // go to attackState
                // _patrol.SetDestination(other.transform.position, 0.5f, _playerDetected); 
            } 
            
            // attack 
            transform.Translate((transform.position - other.transform.position).normalized * Time.fixedTime * 2f, Space.Self);
            // blockedByWall = Physics.Raycast(transform.position, other.transform.position - transform.position, out RaycastHit hit, 15f, props); 
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player") || !_patrol.playerDetected) return;
            _patrol.SetDestination(Vector3.zero, 2f, false); // reset speed internally because you don't know initial value here 
            _notified = false;  
        }
    }
}
