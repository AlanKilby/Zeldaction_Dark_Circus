using UnityEngine;
using MonsterLove.StateMachine;
using UnityEngine.AI;
using System.Collections;
using Random = System.Random;

namespace BEN.AI 
{
    [RequireComponent(typeof(BoxCollider))]
    [DefaultExecutionOrder(5)] 

    public class CheckSurroundings : MonoBehaviour
    {
        [SerializeField] private LayerMask detectableTargetsLayer;
        [SerializeField] private LayerMask wall;
        [SerializeField] private LayerMask player;
        [SerializeField] private LayerMask playerWeapon;
        private BoxCollider _selfCollider;
        private Collider[] _detectedCollidersArray; 
        private FsmPatrol _patrol;
        private NavMeshAgent _agent; 
        private bool _playerDetected;
        private bool _playerIsClosest;
        private RaycastHit[] _detectedColliders; 

        private float[] _distances; 
        private float _smallestValue; 

        private bool _notified; // DEBUG 
        private BasicAIBrain _brain; // NOT SAFE

        public AIType BearerType;
        public bool CanDodgeProjectile { get; set; } 
        public bool IsDead { get; set; }
        private Vector3 playerPosition;
        private Vector3 direction;
        private bool wallIsHidingPlayer; 
        
        public bool LeftWallDetected { get; set; } 
        public bool RightWallDetected { get; set; }

        private Vector3 _dodgeDirection; 
        public Vector3 DodgeDirection { get; private set; }
        private Vector3 _localLeftDirection, _localRightDirection; 


        public CheckSurroundings(Collider[] detectedCollidersArray)
        {
            _detectedCollidersArray = detectedCollidersArray;
        }

        private void Start()
        {
            IsDead = false;
            LeftWallDetected = RightWallDetected = false; 
            
            _selfCollider = GetComponent<BoxCollider>();
            // _patrol = GetComponentInParent<FsmPatrol>(); 
            _agent = GetComponentInParent<NavMeshAgent>();
            _brain = GetComponentInParent<BasicAIBrain>();
            BearerType = GetComponentInParent<BasicAIBrain>().Type;
            CanDodgeProjectile = true; 
        }

        private void OnTriggerEnter(Collider other)
        {
            Boomerang.s_SeenByEnemy = Mathf.Pow(2, other.gameObject.layer) == playerWeapon;

            if (Mathf.Pow(2, other.gameObject.layer) != player) return; 
            
            _playerDetected = true;
            StopCoroutine(nameof(CallDefaultStateAfterDelay));
        } 

        private void OnTriggerStay(Collider other)
        {
            if (!_playerDetected || IsDead) return; 
            playerPosition = other.gameObject.transform.position; 
            
            if (Mathf.Pow(2, other.gameObject.layer) == playerWeapon && BearerType == AIType.MonkeySurBall && CanDodgeProjectile)
            {
                if (Vector3.Distance(other.transform.position, transform.parent.position) > 5f) return; 

                try 
                {
                    _localLeftDirection = transform.InverseTransformDirection(Vector3.left);
                    _localRightDirection = transform.InverseTransformDirection(Vector3.right);
                    
                    LeftWallDetected = Physics.Raycast(transform.position, _localLeftDirection, _brain.MonkeyBallDodgeDistance, wall);
                    RightWallDetected = Physics.Raycast(transform.position, _localRightDirection, _brain.MonkeyBallDodgeDistance, wall); 

                    Debug.DrawRay(transform.position, _localLeftDirection, Color.yellow, 5f); 
                    Debug.DrawRay(transform.position, _localRightDirection, Color.red, 5f);   
                    
                    Debug.Log("before return dodge");

                    if (LeftWallDetected && RightWallDetected) return;
                     
                    Debug.Log("after return dodge");
                    var directionRandomSelector = UnityEngine.Random.Range(0, 2) == 0 ? _localLeftDirection : _localRightDirection;
                    DodgeDirection = LeftWallDetected && RightWallDetected ? directionRandomSelector : (LeftWallDetected ? _localLeftDirection : _localRightDirection);
                    transform.position += DodgeDirection;  
                    
                    _brain.OnRequireStateChange(States.Defend, StateTransition.Safe); 
                }
                catch (System.Exception e) { Debug.Log(e.Message); }
            } 
            
            _detectedColliders = Physics.RaycastAll(transform.position, other.transform.position - transform.position, 
                                                    15f, detectableTargetsLayer);
            // _distances = new float[_detectedColliders.Length];
            
            wallIsHidingPlayer = Physics.Raycast(transform.position, (playerPosition - transform.position).normalized, 
                                                 Vector3.Distance(transform.position, playerPosition), wall);
            // if a wall is hiding the player, return
            if (wallIsHidingPlayer || _notified) return; 

            _brain.TargetToAttackPosition = playerPosition; 
            _brain.OnRequireStateChange(States.Attack, StateTransition.Safe);
            _notified = true; 
            // _patrol.SetDestination(other.transform.position, 0.5f, _playerDetected); if have FSMpatrol 

            // check if player is not behind a wall
            /* for (var i = 0; i < _detectedColliders.Length; i++)
            {
                if( i == 0)
                    _smallestValue = _detectedColliders[i].distance;  
                else if (_smallestValue > _detectedColliders[i].distance)
                {
                    _smallestValue = _detectedColliders[i].distance; 
                }

                _distances[i] = _detectedColliders[i].distance;

                // if (Mathf.Pow(2, _detectedColliders[i].transform.gameObject.layer) != player) return; 
                // works even if player is behind his projectile
                _playerIsClosest = Mathf.Approximately(_smallestValue, _detectedColliders[i].distance); 

                if (!_playerDetected || _notified) continue;
                 
                // go to attackState  
                if (!_notified && other) 
                { 
                    _brain.TargetToAttackPosition = other.transform.position;
                    _brain.OnRequireStateChange(States.Attack, StateTransition.Safe); 
                } 
                _notified = true; 
                // _patrol.SetDestination(other.transform.position, 0.5f, _playerDetected); if have FSMpatrol 
            } */
            
            // attack 
            // blockedByWall = Physics.Raycast(transform.position, other.transform.position - transform.position, out RaycastHit hit, 15f, props); 
        } 

        private void OnTriggerExit(Collider other) 
        {
            if (IsDead || Mathf.Pow(2, other.gameObject.layer) != player || BearerType == AIType.Mascotte) return;
            Boomerang.s_SeenByEnemy = Mathf.Pow(2, other.gameObject.layer) == playerWeapon;

            if (Mathf.Pow(2, other.gameObject.layer) == player)
            {
                _playerDetected = _notified = false;  
            }
 
            StartCoroutine(nameof(CallDefaultStateAfterDelay));  
        } 

        IEnumerator CallDefaultStateAfterDelay() 
        {
            yield return new WaitForSeconds(_brain.DelayBeforeBackToDefaultState);
            _brain.GoingBackToPositionBeforeIdling = true; 
            _brain.OnRequireStateChange(States.Default, StateTransition.Safe); 
        }
    }
}
