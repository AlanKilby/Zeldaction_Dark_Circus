using UnityEngine;
using MonsterLove.StateMachine;
using UnityEngine.AI;
using System.Collections; 

namespace BEN.AI 
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

        private AIType bearerType;
        public bool CanDodgeProjectile { get; set; } 
        public bool IsDead { get; set; }

        public CheckSurroundings(Collider[] detectedCollidersArray)
        {
            _detectedCollidersArray = detectedCollidersArray;
        }

        private void Start()
        {
            IsDead = false; 
            
            _selfCollider = GetComponent<BoxCollider>();
            // _patrol = GetComponentInParent<FsmPatrol>(); 
            _agent = GetComponentInParent<NavMeshAgent>();
            _brain = GetComponentInParent<BasicAIBrain>();
            bearerType = GetComponentInParent<BasicAIBrain>().Type;
            CanDodgeProjectile = true; 
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                StopCoroutine(nameof(CallDefaultStateAfterDelay)); 
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (IsDead) return; 
            
            if (other.CompareTag("PlayerWeapon") && bearerType == AIType.MonkeySurBall && CanDodgeProjectile) 
            {
                try
                {
                    _brain.OnRequireStateChange(States.Defend, StateTransition.Safe); 
                }
                catch (System.Exception e) { Debug.Log(e.Message); }
            } 

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

                if (!_detectedColliders[i].transform.gameObject.CompareTag("Player")) return;
                // player is detected if his collider is the closest to enemy
                _playerDetected = Mathf.Approximately(_smallestValue, _detectedColliders[i].distance);

                if (!_playerDetected || _notified) continue;
                
                // go to attackState  
                if (!_notified && other) 
                { 
                    _brain.TargetToAttackPosition = other.transform.position;
                    _brain.OnRequireStateChange(States.Attack, StateTransition.Safe); 
                } 
                _notified = true; 
                // _patrol.SetDestination(other.transform.position, 0.5f, _playerDetected); if have FSMpatrol 
            } 
            
            // attack 
            // blockedByWall = Physics.Raycast(transform.position, other.transform.position - transform.position, out RaycastHit hit, 15f, props); 
        } 

        private void OnTriggerExit(Collider other) 
        {
            if (IsDead) return;

            if (!other.CompareTag("Player") || bearerType == AIType.Mascotte) return; // mascotte follows players for ever once detected 

            _notified = false;
            StartCoroutine(nameof(CallDefaultStateAfterDelay));  
        } 

        IEnumerator CallDefaultStateAfterDelay() 
        {
            yield return new WaitForSeconds(_brain.DelayBeforeBackToDefaultState);
            _brain.GoingBackToPositionBeforeIdling = true; 
            _brain.OnRequireStateChange(States.Default, StateTransition.Safe); 
        }

        private bool IsFacingProjectile(Vector3 projectile) => (Mathf.Sign(Vector3.Dot(transform.position, projectile)) > 0); 
    }
}
