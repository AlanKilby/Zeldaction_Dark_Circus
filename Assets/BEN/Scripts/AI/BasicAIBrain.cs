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
        Clear
    } 
    
    [RequireComponent(typeof(NavMeshAgent))]
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
        [SerializeField, ConditionalShow("isCaster", true), Tooltip("Wait time between each attack for MSB and Fakir"), Range(2f, 8f)] private float attackRate = 4f;
        [SerializeField, ConditionalShow("isMonkeyBall", true), Tooltip("Delay before jumping back to ball again"), Range(2f, 5f)] private float monkeyBallProvocDuration = 3f;
        [SerializeField, Tooltip("DefaultSpeed increse when rushing toward the player. 1 = no increase"), Range(1f, 3f)] private float attackStateSpeedMultiplier = 1.25f;
        [SerializeField, Tooltip("Delay from Idle to Attack State when player is detected"), Range(0f, 5f)] private float attackDelay = 1f; 
        [SerializeField, Range(1f, 30f)] private float attackRange = 1f;   

        private StateMachine<States> _fsm; 
        private GameObject _patrolZone; 
        private GameObject _ball; 
        [SerializeField] private GameObject _graphics;
        
        private FsmPatrol _patrol;
        
        private NavMeshAgent _agent; 
        public bool playerDetected; // DEBUG

        public static Action<States, StateTransition> OnRequireStateChange;
        public Vector3 TargetToAttackPosition { get; set; }

        private AIAnimation _aIAnimation;
        private AIAnimation _ballAnimation; 

        // private Vector3 _positionBeforeAttacking; // a node or single position 
        private Animator _ballAnimator = null; 
        private const int _moveRight = 1;

        private Sprite _sprite;

        private CheckSurroundings _checkSurroundings; 

        public bool HasBeenInvokedByBoss { get; set; } 

        [Header("-- DEBUG --")]
        [SerializeField] private EditorDebuggerSO debugger;
        [SerializeField] public bool refresh;
        private Collider monkeyBallCollider;
        private Collider ballCollider;

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
            if (!HasBeenInvokedByBoss)
            {
                _patrol = GetComponent<FsmPatrol>();
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
        } 

        private void FixedUpdate() 
        {
            Debug.Log("rotation index is " + Mathf.FloorToInt(transform.rotation.eulerAngles.y / 90)); // use this to set anim direction
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

#region FSM

        #region Init 

        void Init_Enter()
        {
            Debug.Log("Initializing Default State"); 
            _aIAnimation = _graphics.GetComponent<AIAnimation>();
            _fsm.ChangeState(States.Default, StateTransition.Safe);
        }

        void Init_Exit()
        {
            Debug.Log("Transition to default state");
        }
        
        #endregion  
        
        #region Default
        IEnumerator Default_Enter()  
        { 
            Debug.Log("entering default state");
            yield return new WaitForSeconds(0.04f); 

            // idle when waiting at a patrol node point 

            /* switch (type)
            {
                case AIType.Monkey: 
                    _aIAnimation.PlayAnimation(AnimState.WalkRight); // make it dynamic direction instead 
                    break;
                case AIType.MonkeySurBall:
                    _aIAnimation.PlayAnimation(AnimState.WalkRight); // make it dynamic direction instead
                    // _ballAnimation.PlayAnimation(); 
                    break;
                case AIType.Mascotte: 
                    _aIAnimation.PlayAnimation(AnimState.WalkLeft); // make it dynamic direction instead  
                    break;
                case AIType.Fakir:
                    _aIAnimation.PlayAnimation(AnimState.WalkRight); // make it dynamic direction instead  
                    break;
            } */
        } 

        void Default_FixedUpdate() 
        { 
            Debug.Log("updating default state");
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
            Debug.Log("exiting default state");
            //Reset object to desired configuration
        }
        
        void Default_Finally()
        {
            Debug.Log("finally of default state");
            //Reset object to desired configuration
        } 
        
        #endregion 

        #region Attack
        
        private IEnumerator Attack_Enter() // UPGRADE : use async-await coroutines
        {
            yield return new WaitForSeconds(attackDelay); 
            Debug.Log($"Attacking in {attackDelay} seconds"); 
            
            _agent.destination = TargetToAttackPosition;
            _agent.speed = 0f; 

            // make the enemy predict the future player position instead of aiming at it's current one
            switch (type) 
            {
                case AIType.Monkey:
                    Debug.Log("Monkey => attacking");
                    // _aIAnimation.PlayAnimation(AnimState.AtkRight); 
                    break;
                case AIType.MonkeySurBall:
                    Debug.Log("MonkeyBall => attacking");
                    // _aIAnimation.PlayAnimation(AnimState.AtkRight);
                    InvokeRepeating(nameof(MonkeyBallAttack), 0f, attackRate); 
                    break;
                case AIType.Mascotte:
                    Debug.Log("Mascotte => attacking");
                    // _aIAnimation.PlayAnimation(AnimState.AtkLeft);
                    break;
                case AIType.Fakir:
                    Debug.Log("Fakir => attacking"); 
                    // _aIAnimation.PlayAnimation(AnimState.AtkRight);
                    _agent.speed = 0f;
                    InvokeRepeating(nameof(FakirAttack), 0f, attackRate); 
                    break;
                default:
                    Debug.Log("Default => breaking");
                    break; 
            } 
            // abstracted away => this should be standard // make it dynamic direction instead 
        }

        private void Attack_FixedUpdate()  
        {
            if (Vector3.Distance(transform.position, PlayerMovement_Alan.sPlayerPos) <= attackRange) 
            {
                _agent.speed = 0f;  
            } 
            else 
            {
                _agent.speed = defaultSpeed * attackStateSpeedMultiplier; 
                _agent.destination = PlayerMovement_Alan.sPlayerPos;
            }

            Debug.Log("Attacking fixedUpdate");
        } 
 
        private void MonkeyBallAttack()
        {
            GameObject reference = Instantiate(_monkeyBallProjectile, _graphics.transform.position, _graphics.transform.rotation);
            reference.transform.position = _graphics.transform.position; 
        }

        private void FakirAttack() // WARNING : Duplicate
        {
            GameObject reference = Instantiate(_fakirProjectile, _graphics.transform.position, _graphics.transform.rotation);
            reference.transform.position = _graphics.transform.position;
        }

        void Attack_Exit()
        {
            if (HasBeenInvokedByBoss)
            {
                TransitionToNewState(States.Attack, StateTransition.Overwrite); // debug crados             
            }

            Debug.Log("Attacking exit");
            _agent.destination = _patrol.Points[_patrol.DestPoint].position; // TODO : use closest point of list instead
            _agent.speed = defaultSpeed / attackStateSpeedMultiplier; 
            // _aIAnimation.PlayAnimation(AnimState.WalkRight); // make it dynamic direction instead 
                                                                  // _ballAnimation.StopAnimating(); only when initial position is reached

            switch (type)
            {
                case AIType.MonkeySurBall: 
                    Debug.Log("MonkeyBall => exit attack");
                    CancelInvoke(nameof(MonkeyBallAttack)); 
                    break;
                case AIType.Fakir:
                    Debug.Log("Fakir => exit attack");
                    CancelInvoke(nameof(FakirAttack)); 
                    break; 
            } 
        }

        #endregion

        #region Defend

        IEnumerator Defend_Enter()
        { 
            Debug.Log("defend enter");
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
            Debug.Log("Defend_Exit");
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

#endregion
    }
}
