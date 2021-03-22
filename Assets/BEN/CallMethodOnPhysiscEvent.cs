using BEN.Scripts;
using UnityEngine;

// TODO : extend it so it can be called on any type of collider (box and sphere mostly)
namespace BEN
{
    [RequireComponent(typeof(BoxCollider))] // DEBUG
    public class CallMethodOnPhysiscEvent : MonoBehaviour
    {
        [SerializeField, Range(1, 5)] private int radius = 1;
        [SerializeField] private LayerMask detectableTargetsLayer;
        private BoxCollider _selfCollider;
        private Collider[] _detectedCollidersArray;
        private AgentPatrol _patrolBehaviour;
        private bool _playerDetected;
        private RaycastHit[] _detectedColliders;

        private float[] _distances;
        private float _smallestValue;

        private bool _notified; // DEBUG 
        public bool isMascotte = false; 

        public CallMethodOnPhysiscEvent(Collider[] detectedCollidersArray)
        {
            this._detectedCollidersArray = detectedCollidersArray;
        }

        private void Start() 
        {
            _selfCollider = GetComponent<BoxCollider>();
            _patrolBehaviour = GetComponentInParent<AgentPatrol>(); 
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
            var position = transform.position; // cf doc https://github.com/JetBrains/resharper-unity/wiki/Avoid-multiple-unnecessary-property-accesses 
            Debug.DrawRay(position, other.transform.position - position, Color.red); 

            // rush toward player => move elsewhere
            if (other.CompareTag("Player") && !_patrolBehaviour.playerDetected && _playerDetected)
            {
                _patrolBehaviour.SetDestination(other.transform.position, 0f + Utility.BoolToInt(isMascotte), _playerDetected); 
            }
        }

        private void OnTriggerStay(Collider other)
        {
            // exposed native data is unnefficient 
            Debug.DrawRay(transform.position, other.transform.position - transform.position, Color.red);
            
            // use RaycastNonAlloc instead !!
            _detectedColliders = Physics.RaycastAll(transform.position, other.transform.position - transform.position, 15f, detectableTargetsLayer);
            _distances = new float[_detectedColliders.Length];

            // keep moving toward player
            for (var i = 0; i < _detectedColliders.Length; i++)
            {
                // check if player is behind a wall (smallest value will be = to a wall collider)
                if( i == 0)
                    _smallestValue = _detectedColliders[i].distance; 
                else if (_smallestValue > _detectedColliders[i].distance)
                {
                    _smallestValue = _detectedColliders[i].distance; 
                }

                _distances[i] = _detectedColliders[i].distance;

                // if player is the closest (even when a wall is detected behind player)
                if (!_detectedColliders[i].transform.gameObject.CompareTag("Player")) continue;
                _playerDetected = Mathf.Approximately(_smallestValue, _detectedColliders[i].distance);

                if (!_playerDetected || !_notified) continue;
                _notified = true; 
                
                // attack player from distance or rush toward it 
                _patrolBehaviour.SetDestination(other.transform.position, 0f + Utility.BoolToInt(isMascotte), _playerDetected);
                
            } 
            // blockedByWall = Physics.Raycast(transform.position, other.transform.position - transform.position, out RaycastHit hit, 15f, props);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player") || !_patrolBehaviour.playerDetected) return;
            _patrolBehaviour.SetDestination(Vector3.zero, 2f, false); // reset speed internally because you don't know initial value here 
            _notified = false;
        }
    }
}
