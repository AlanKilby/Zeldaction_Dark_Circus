using System;
using System.Collections;
using System.Collections.Generic; 
using UnityEngine;
using MonsterLove.StateMachine;
using Debug = UnityEngine.Debug;
using UnityEngine.AI;
#if UNITY_EDITOR
using UnityEditor;
#endif
using BEN.Animation;
using BEN.Math; 


/* 
 Very simple architecture (enum-based FSM) to avoid multiple classes and costly virtual calls
 
 All calls and processing are made from here with a switch based on AIType
 
 Upgrade to class-based FSM only if needed
 */

namespace BEN.AI 
{
    public enum AIType
    { 
        Monkey, 
        MonkeySurBall,
        Mascotte, 
        Fakir  
    }
    
    public enum States
    {
        Init, 
        Default,  
        Attack,
        Defend,  
        Die
    } 
    
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Health))]
    [DefaultExecutionOrder(10)] 
    public class BasicAIBrain : MonoBehaviour
    {
        [SerializeField] private AIType type;
        public AIType Type { get => type; set => Type = value; } 
        
        // used for conditionalShow's property drawer until I know how to directly use enum 
        [HideInInspector] public bool isMonkeyBall;
        [HideInInspector] public bool isFakir;
        [HideInInspector] public bool isCaster; 

        [SerializeField, ConditionalShow("isMonkeyBall", true)] private GameObject _monkeyBallProjectile;
        [SerializeField, ConditionalShow("isFakir", true)] private GameObject _fakirProjectile; 
        [SerializeField, Tooltip("Speed when patrolling"), Range(0.5f, 5f)] private float defaultSpeed = 2f;
        [SerializeField, Range(0.5f, 5f), Tooltip("Wait time between each attack")] private float attackRate = 2f;
        [SerializeField, ConditionalShow("isMonkeyBall"), Tooltip("Delay before jumping back to ball again")] private float monkeyBallProvocDuration = 3f;
        [SerializeField, Tooltip("DefaultSpeed increse when rushing toward the player. 1 = no increase"), Range(1f, 3f)] private float attackStateSpeedMultiplier = 1.25f;
        [SerializeField, Tooltip("Delay from Idle to Attack State when player is detected"), Range(0f, 5f)] private float attackDelay = 1f; 
        [SerializeField, Range(1f, 30f)] private float attackRange = 1f;
        [SerializeField, Range(1, 5)] private sbyte attackDamage = 1;

        private StateMachine<States> _fsm;
        private GameObject _ball; 
        [SerializeField] private GameObject _graphics; // MOVE TO AIANIMATION
        [SerializeField] private GameObject _detection;
        private Health _agentHp;

        private FsmPatrol _patrol;
        
        private NavMeshAgent _agent; 

        public Action<States, StateTransition> OnRequireStateChange; 
        public Vector3 TargetToAttackPosition { get; set; }

        private AIAnimation _aIAnimation; // MOVE TO AIANIMATION
        private AIAnimation _ballAnimation; // MOVE TO AIANIMATION

        // private Vector3 _positionBeforeAttacking; // a node or single position 

        private CheckSurroundings _checkSurroundings; 
        
        private AnimDirection _animDirection; // MOVE TO AIANIMATION
        private int _previousParentRotation; // MOVE TO AIANIMATION
        
        public bool HasBeenInvokedByBoss { get; set; }
        [SerializeField] private PlaceholderDestination _placeholderDestination;
        private Health _playerHP; 


        [Header("-- DEBUG --")]
        [SerializeField] private EditorDebuggerSO debugger;
        [SerializeField] public bool refresh;
        private Collider monkeyBallCollider;
        private Collider ballCollider;
        public float angle;
        private bool hasCalledFakeCAC; 

        #region Editor

        #endregion

#region Unity Callbacks

        private void Awake()
        {
            _fsm = StateMachine<States>.Initialize(this);
            _fsm.ChangeState(States.Init, StateTransition.Safe);
        }

        private void OnEnable() 
        {
            OnRequireStateChange += TransitionToNewState;
        } 
        
        private void OnValidate()
        {
            isFakir = type == AIType.Fakir;
            isMonkeyBall = type == AIType.MonkeySurBall;
            isCaster = isFakir || isMonkeyBall; 
        }

        private void OnDrawGizmos() 
        {
            try
            {
                for (var i = 0; i < _patrol.Points.Length; i++)
                {
                    if (i == 0)
                    {
                        Gizmos.color = Color.yellow;
                    }
                    else if (Type == AIType.Monkey)
                    {
                        Gizmos.color = Color.white;
                    }
                    else if (Type == AIType.MonkeySurBall)
                    {
                        Gizmos.color = Color.black;
                    }
                    else if (Type == AIType.Mascotte)
                    {
                        Gizmos.color = Color.blue;
                    }
                    else
                    {
                        Gizmos.color = Color.red;
                    }
                    
                    Gizmos.DrawLine(_patrol.Points[i].position, _patrol.Points[(int) Mathf.Repeat(i + 1, _patrol.Points.Length)].position);
                    Gizmos.DrawWireSphere(_patrol.Points[i].position, 0.25f);
                }
            }
            catch (Exception e)
            {
                Debug.Log($"{ e.Message} thrown by {gameObject.name}"); 
                _patrol = GetComponent<FsmPatrol>();
                _patrol.SetPoints();
            } 
        }

        private void Start()
        {
            _playerHP = PlayerMovement_Alan.sPlayer.GetComponentInChildren<Health>(); 
            _agentHp = GetComponent<Health>(); 

            if (!HasBeenInvokedByBoss)
            {
                _patrol = GetComponent<FsmPatrol>();
                _patrol.SetPoints(); // debug 
            }

            _agent = GetComponent<NavMeshAgent>();
            _checkSurroundings = GetComponentInChildren<CheckSurroundings>();

            if (Type == AIType.MonkeySurBall)
            {
                monkeyBallCollider = GetComponent<BoxCollider>();
                ballCollider = GetComponentInChildren<SphereCollider>();

                if (!_ball)
                {
                    try
                    {
                        _ball = transform.GetChild(2).gameObject;
                    }
                    catch (Exception e) { Debug.Log(e.Message); } 
                }
            }

            _agent.speed = defaultSpeed;
            _previousParentRotation = _placeholderDestination.angleIndex; 
        } 

        private void FixedUpdate()
        {
            _checkSurroundings.transform.rotation = Quaternion.Euler(0f, _placeholderDestination.angle, 0f);
            CheckAnimDirection();

            if (_agentHp.CurrentValue <= 0 && !_patrol.IsDead)
            {
                Debug.Log("transition to death state"); 
                OnRequireStateChange(States.Die, StateTransition.Safe); 
            }
        }

        private void OnDisable() 
        {
            OnRequireStateChange -= TransitionToNewState;
        } 
        
        
#endregion 

        // called by event OnRequireStateChange
        private void TransitionToNewState(States newState, StateTransition transition) 
        {
            _fsm.ChangeState(newState, transition); 
        }
        
        // MOVE ALL THIS TO AIANIMATION ===>

        private void CheckAnimDirection()
        {
            _animDirection = (AnimDirection) (_placeholderDestination.angleIndex);

            if (_placeholderDestination.angleIndex == _previousParentRotation) return;
            
            _aIAnimation.PlayAnimation(AnimState.Walk, _animDirection);
            _previousParentRotation = _placeholderDestination.angleIndex;
            StartCoroutine(nameof(ChangeGraphicsRotation)); 
        } 
        
        private void CheckAnimDirection(AnimState state)
        {
            _animDirection = (AnimDirection) (_placeholderDestination.angleIndex);

            if (_placeholderDestination.angleIndex == _previousParentRotation) return;
            
            _aIAnimation.PlayAnimation(state, _animDirection);
            
            _previousParentRotation = _placeholderDestination.angleIndex;
            StartCoroutine(nameof(ChangeGraphicsRotation)); 
        } 

        private IEnumerator ChangeGraphicsRotation()
        {
            yield return new WaitForSeconds(1.5f);
            _graphics.transform.localRotation = Quaternion.identity; 
        }
        
        // <===

#region FSM

        #region Init 

        void Init_Enter()
        {
            _aIAnimation = _graphics.GetComponent<AIAnimation>();
            _fsm.ChangeState(States.Default, StateTransition.Safe);
        }

        void Init_Exit()
        {
        }
        
        #endregion  
        
        #region Default
        IEnumerator Default_Enter()  
        { 
            yield return new WaitForSeconds(0.03f);
            _aIAnimation.PlayAnimation(AnimState.Walk, _animDirection); 
        }

        void Default_FixedUpdate() 
        { 
            switch (type)
            {
                case AIType.Monkey: 
                    break;
                case AIType.MonkeySurBall:
                    break;
                case AIType.Mascotte:
                    break;
                case AIType.Fakir:
                    break; 
                default:
                    break;
            }
        } 
        
        void Default_Exit()
        {
            //Reset object to desired configuration
        }
        
        #endregion 

        #region Attack
        
        private IEnumerator Attack_Enter() // UPGRADE : use async-await coroutines
        {
            yield return new WaitForSeconds(attackDelay); 
            
            _agent.destination = TargetToAttackPosition;

            // UPGRADE : make the enemy predict the future player position instead of aiming at it's current one
            switch (type) 
            {
                case AIType.Monkey:
                    _aIAnimation.PlayAnimation(AnimState.Atk, _animDirection);
                    break;
                case AIType.MonkeySurBall:
                    _aIAnimation.PlayAnimation(AnimState.Atk, _animDirection);
                    InvokeRepeating(nameof(MonkeyBallAttack), 0f, attackRate); 
                    break;
                case AIType.Mascotte:
                    _aIAnimation.PlayAnimation(AnimState.Atk, _animDirection);
                    break;
                case AIType.Fakir:
                    _aIAnimation.PlayAnimation(AnimState.Atk, _animDirection);
                    _agent.speed = 0f;
                    InvokeRepeating(nameof(FakirAttack), 0f, attackRate); 
                    break; 
            } 
        }

        private void Attack_FixedUpdate()  
        {
            if (Vector3.Distance(transform.position, PlayerMovement_Alan.sPlayerPos) <= attackRange) 
            {
                _agent.speed = 0f;
                
                // to simulate player killed from CAC. Distance is done from projectile
                if ((type == AIType.Monkey || type == AIType.Mascotte) && !hasCalledFakeCAC) 
                {
                    hasCalledFakeCAC = true;
                    InvokeRepeating(nameof(FakeCAC), 0.5f, attackRate); 
                } 
            } 
            else 
            {
                _agent.speed = defaultSpeed * attackStateSpeedMultiplier; 
                _agent.destination = PlayerMovement_Alan.sPlayerPos;
                CheckAnimDirection(AnimState.Atk); 
            } 
        } 

        private void FakeCAC()
        {
            _playerHP.DecreaseHp(attackDamage); 
        }

        private void MonkeyBallAttack()
        {
            GameObject reference = Instantiate(_monkeyBallProjectile, _graphics.transform.position, _detection.transform.rotation);
            reference.transform.position = _graphics.transform.position;
            Debug.Log("monkeyball projectile");
        }

        private void FakirAttack() // WARNING : Duplicate 
        {
            GameObject reference = Instantiate(_fakirProjectile, _graphics.transform.position, _detection.transform.rotation);
            reference.transform.position = _graphics.transform.position;
            reference.GetComponent<ParabolicFunction>().CasterTransform = _graphics.transform; 
            Debug.Log("fakir projectile");

        }

        void Attack_Exit()
        {
            if (HasBeenInvokedByBoss)
            {
                TransitionToNewState(States.Attack, StateTransition.Overwrite); // debug crados             
            }

            _agent.destination = _patrol.Points[_patrol.DestPoint].position; // TODO : use closest point of list instead
            _agent.speed = defaultSpeed / attackStateSpeedMultiplier; 
            // _aIAnimation.PlayAnimation(AnimState.WalkRight); // make it dynamic direction instead 
                                                                  // _ballAnimation.StopAnimating(); only when initial position is reached

            switch (type)
            {
                case AIType.MonkeySurBall: 
                    CancelInvoke(nameof(MonkeyBallAttack)); 
                    break;
                case AIType.Fakir:
                    CancelInvoke(nameof(FakirAttack)); 
                    break; 
            }

            hasCalledFakeCAC = false; 
        }

        #endregion

        #region Defend

        IEnumerator Defend_Enter()
        { 
            _agent.speed = 0f;
            _graphics.transform.localPosition = Vector3.zero; 
            _ball.SetActive(false);
            // _aIAnimation.PlayAnimation(11); 
            _checkSurroundings.CanDodgeProjectile = false;
            monkeyBallCollider.enabled = false;
            ballCollider.enabled = false;

            yield return new WaitForSeconds(1f);
            monkeyBallCollider.enabled = true;
            ballCollider.enabled = true;

            yield return new WaitForSeconds(monkeyBallProvocDuration - 1f); 
            OnRequireStateChange(States.Attack, StateTransition.Safe); 
        }

        void Defend_Exit() 
        { 
            _agent.speed = defaultSpeed;
            _graphics.transform.localPosition = Vector3.up; 
            _ball.SetActive(true);
            // _aIAnimation.PlayAnimation(AnimState.AtkRight); 

            Invoke(nameof(ResetBool), 8f); 
        }

        void ResetBool()
        {
            _checkSurroundings.CanDodgeProjectile = true; 
        }
        #endregion

        #region Die
        IEnumerator Die_Enter() 
        {
            _patrol.IsDead = _checkSurroundings.IsDead = true; // DEBUG
            Debug.Log("die_enter"); 
            
            yield return new WaitForSeconds(0.25f);  
            CancelInvoke();

            try
            {
                _aIAnimation.PlayAnimation(AnimState.Hit, AnimDirection.None);
            }
            catch (Exception e) 
            {
                Debug.Log($"{e.Message }. \n callind Die state instead of Hit state"); 
                _aIAnimation.PlayAnimation(AnimState.Die, AnimDirection.None);
            }
        } 
        
        #endregion

#endregion
    }
}
