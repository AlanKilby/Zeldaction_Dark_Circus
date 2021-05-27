using System;
using System.Collections;
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
 
 Separate flow-control and behaviours if needed
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
#region Serialized Variables

        [SerializeField] private AIType type;
        [SerializeField] private bool _canPatrol = true; 
        public AIType Type { get => type; set => Type = value; } 
        
        // used for conditionalShow's property drawer until I know how to directly use enum 
        [HideInInspector] public bool isMonkeyBall;
        [HideInInspector] public bool isFakir; 
        [HideInInspector] public bool isCaster; 
        
        [SerializeField, ConditionalShow("isFakir", true)] private GameObject _fakirProjectile; 
        [SerializeField, ConditionalShow("isMonkeyBall", true)] private GameObject _monkeyBallProjectile;
        
        [Header("General")]
        [Space, SerializeField, Tooltip("Speed when patrolling"), Range(0f, 5f)] private float _defaultSpeed = 2f;
        [SerializeField, Range(0.5f, 5f), Tooltip("Wait time between each attack")] private float _attackRate = 2f;
        [SerializeField, Tooltip("DefaultSpeed increase when rushing toward the player. 1 = no increase"), Range(1f, 3f)] private float _attackStateSpeedMultiplier = 1.25f;
        [SerializeField, Tooltip("Delay from Default to Attack State when player is detected"), Range(0f, 5f)] private float _attackDelay = 1f; 
        [SerializeField, Range(1f, 30f)] private float _attackRange = 1f; 
        [SerializeField, Range(1, 5)] private sbyte _attackDamage = 1;
        [SerializeField, Range(0f, 5f), Tooltip("Delay from Attack to Default State when player is not detected anymore")] private float _delayBeforeBackToDefaultState = 3f;
        [SerializeField, Range(1f, 20f), Tooltip("Radius of ally notification when player is detected. Notified allies start chasing the player too")] private float _allyNotifyRadius = 8f; 
        
        [Header("Specific")]
        [Space, SerializeField, ConditionalShow("isMonkeyBall"), Tooltip("Delay before jumping back to ball again")] private float _monkeyBallProvocDuration = 3f;
        [SerializeField, ConditionalShow("isMonkeyBall")] private float _monkeyBallDodgeReactionTime = 0.5f;
        [SerializeField, ConditionalShow("isMonkeyBall"), Tooltip("how far monkey ball goes when dodging")] private float _monkeyBallDodgeDistance = 5f;

        [Header("Other")] 
        [Space, SerializeField] private GameObject _graphics; // MOVE TO AIANIMATION
        [SerializeField] private GameObject _detection;
        [SerializeField] private PlaceholderDestination _placeholderDestination;
        [SerializeField, ConditionalShow("isMonkeyBall", true)] private GameObject _ballGraphics;
        [SerializeField] private Behaviour[] _componentsToDeactivateOnDeath; 

        [Header("-- DEBUG --")]
        [SerializeField] private EditorDebuggerSO _debugger;
        [SerializeField] private bool refresh;
        private bool wasMonkeyBall; 
        
#endregion 

#region Public Variables
        public Action<States, StateTransition> OnRequireStateChange;
#endregion

#region Private & Protected Variables

    private StateMachine<States> _fsm;
    private Health _agentHp;
    private FsmPatrol _patrol;
    private NavMeshAgent _agent; 
    
    private AIAnimation _aIAnimation; // MOVE TO AIANIMATION
    private AIAnimation _ballAnimation; // MOVE TO AIANIMATION + not used

    private Vector3 _idlePositionBeforeAttacking; // when not patrolling

    private CheckSurroundings _checkSurroundings; 
        
    private AnimDirection _animDirection; // MOVE TO AIANIMATION
    private int _previousParentRotation; // MOVE TO AIANIMATION
        
    private Health _playerHP;
    private bool _exitingAttackState;

    private Collider _monkeyBallCollider;
    private Collider _ballCollider; 
    private bool _hasCalledFakeCac;

#endregion

#region Properties

        public float DelayBeforeBackToDefaultState { get ; private set ; } 
        public float DefaultSpeed { get; set; }
        public States NewState { get; private set; }
        public Vector3 TargetToAttackPosition { get; set; }
        public bool GoingBackToPositionBeforeIdling { get; set; }
        public bool HasBeenInvokedByBoss { get; set; }
        public float MonkeyBallDodgeDistance { get; private set; }
        
#endregion
#region Unity Callbacks

        private void Awake()
        {
            _fsm = StateMachine<States>.Initialize(this);
            _fsm.ChangeState(States.Init, StateTransition.Safe); 
            
            _agentHp = GetComponent<Health>();
            _agentHp.IsAI = true; 
        }

        private void OnEnable() 
        {
            OnRequireStateChange += TransitionToNewState;
            _agentHp.OnMonkeyBallTransitionToNormalMonkey += BecomeNormalMonkey; 
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
                    else
                        Gizmos.color = Type switch
                        {
                            AIType.Monkey => Color.white,
                            AIType.MonkeySurBall => Color.black,
                            AIType.Mascotte => Color.blue,
                            _ => Color.red
                        };

                    Gizmos.DrawLine(_patrol.Points[i].position, _patrol.Points[(int) Mathf.Repeat(i + 1, _patrol.Points.Length)].position);
                    Gizmos.DrawWireSphere(_patrol.Points[i].position, 0.25f);
                    
#if  UNITY_EDITOR
                    Handles.color = Color.red; 
                    Handles.DrawWireDisc(transform.position ,Vector3.up, _allyNotifyRadius); 
#endif
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
            DelayBeforeBackToDefaultState = _delayBeforeBackToDefaultState;
            GoingBackToPositionBeforeIdling = false;
            DefaultSpeed = _defaultSpeed; 
            MonkeyBallDodgeDistance = _monkeyBallDodgeDistance; 

            _patrol = GetComponent<FsmPatrol>();
            _patrol.SetPoints(); 

            if (HasBeenInvokedByBoss || !_canPatrol)
            {
                _patrol.enabled = false; 
            }

            _agent = GetComponent<NavMeshAgent>();
            _checkSurroundings = GetComponentInChildren<CheckSurroundings>();

            if (Type == AIType.MonkeySurBall)
            {
                _monkeyBallCollider = GetComponent<BoxCollider>(); 
                _ballCollider = GetComponentInChildren<SphereCollider>();
            }

            _agent.speed = DefaultSpeed;
            _previousParentRotation = _placeholderDestination.angleIndex; 
        } 

        private void FixedUpdate()
        {
            // if (NewState == States.Die) Destroy(transform.parent.gameObject, 1f);  
            
            if (_canPatrol)
            {
                _detection.transform.rotation = Quaternion.Euler(0f, _placeholderDestination.EulerAnglesY, 0f);
            }
            CheckAnimDirection(); // remove from state machine 

            if (_agentHp.CurrentValue <= 0 && !_patrol.IsDead && Type != AIType.MonkeySurBall) 
            {
                Debug.Log("transition to death state"); 
                OnRequireStateChange(States.Die, StateTransition.Safe); 
            }

            if (!_canPatrol && Vector3.Distance(transform.position, _idlePositionBeforeAttacking) <= 0.25f && _exitingAttackState) 
            {
                _exitingAttackState = false; 
                _agent.speed = 0f;
                _aIAnimation.PlayAnimation(AnimState.Idle, AnimDirection.Right); // use AnimDirection according to where you come from . 
            }
        }

        private void OnDisable() 
        {
            OnRequireStateChange -= TransitionToNewState;
            _agentHp.OnMonkeyBallTransitionToNormalMonkey -= BecomeNormalMonkey;
        } 
        
        
#endregion 

        // called by event OnRequireStateChange
        private void TransitionToNewState(States newState, StateTransition transition) 
        {
            _fsm.ChangeState(newState, transition);
            NewState = newState; 
        }
        
        // MOVE ALL THIS TO AIANIMATION ===> WARNING : duplicate 
        private void CheckAnimDirection()
        {
            if (Type == AIType.Fakir && !_canPatrol) return; // modify is fakir needs repositionning 

            _animDirection = (AnimDirection) (_placeholderDestination.angleIndex); 

            if (_placeholderDestination.angleIndex == _previousParentRotation) return;
            
            _aIAnimation.PlayAnimation(AnimState.Walk, _animDirection);
            _previousParentRotation = _placeholderDestination.angleIndex;
            StartCoroutine(nameof(ChangeGraphicsRotation));
        } 
        
        private void CheckAnimDirection(AnimState state)
        {
            // if (Type == AIType.Fakir && !_canPatrol && NewState == States.Attack) return; // modify if fakir needs repositionning 

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
            _fsm.ChangeState(NewState = States.Default, StateTransition.Safe);
            Debug.Log("init_enter");
        }

        void Init_Exit()
        {
        }
        
        #endregion  
        
        #region Default
        IEnumerator Default_Enter()  
        { 
            yield return new WaitForSeconds(0.03f);
            Debug.Log("default_enter");

            if (_canPatrol || GoingBackToPositionBeforeIdling) 
            {
                if (Type == AIType.Fakir && !_canPatrol)
                {
                    _aIAnimation.PlayAnimation(AnimState.Idle, AnimDirection.Right); 
                }
                else
                {
                    _aIAnimation.PlayAnimation(AnimState.Walk, _animDirection);
                }
            } 
            else 
            {
                _aIAnimation.PlayAnimation(AnimState.Idle, AnimDirection.Right);
            }
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
            yield return new WaitForSeconds(_attackDelay); 
            
            _agent.destination = TargetToAttackPosition;
            _idlePositionBeforeAttacking = transform.position;
            _agent.speed = DefaultSpeed;
            Debug.Log("attack_enter");

            // UPGRADE : make the enemy predict the future player position instead of aiming at it's current one
            switch (type) 
            {
                case AIType.Monkey:
                    _aIAnimation.PlayAnimation(AnimState.Atk, _animDirection);
                    break;
                case AIType.MonkeySurBall:
                    _aIAnimation.PlayAnimation(AnimState.Atk, _animDirection); 
                    break;
                case AIType.Mascotte:
                    _aIAnimation.PlayAnimation(AnimState.Walk, _animDirection);
                    break;
                case AIType.Fakir:
                    _aIAnimation.PlayAnimation(AnimState.Atk, _animDirection);
                    _agent.speed = 0f;
                    InvokeRepeating(nameof(FakirAttack), 0f, _attackRate);  
                    break; 
            } 
        } 

        private void Attack_FixedUpdate()  
        {
            if (Vector3.Distance(transform.position, PlayerMovement_Alan.sPlayerPos) <= _attackRange) 
            {
                _agent.speed = 0f;

                if (Type == AIType.Fakir && !_canPatrol && NewState == States.Attack)
                {
                    CheckAnimDirection(AnimState.Atk);
                } 

                // back to default when player has been killed
                if (LoadSceneOnPlayerDeath.playerIsDead)
                {
                    _fsm.ChangeState(States.Default, StateTransition.Safe);
                    CancelInvoke(nameof(FakeCAC));
                } 

                // to simulate player killed from CAC. Distance is done from projectile
                if ((type != AIType.Monkey && type != AIType.Mascotte) || _hasCalledFakeCac) return;
                _hasCalledFakeCac = true;

                if (type == AIType.Mascotte)
                {
                    _aIAnimation.PlayAnimation(AnimState.Atk, _animDirection);  
                }
                 
                InvokeRepeating(nameof(FakeCAC), 0.5f, _attackRate);
            } 
            else 
            {
                _agent.speed = DefaultSpeed * _attackStateSpeedMultiplier; 
                _agent.destination = PlayerMovement_Alan.sPlayerPos;

                CancelInvoke(nameof(FakeCAC));
                _hasCalledFakeCac = false;
                if (type == AIType.Mascotte)
                {
                    _aIAnimation.PlayAnimation(AnimState.Walk, _animDirection);
                }
                else
                {
                    CheckAnimDirection(AnimState.Atk);  
                }
                
            } 
        } 

        private void FakeCAC()
        {
            _playerHP.DecreaseHp(_attackDamage); 
        } 

        private void FakirAttack() 
        {
            _fakirProjectile.GetComponentInChildren<ParabolicFunction>().CasterTransform = _graphics.transform;
            Debug.Log("fakir projectile"); 
        }

        // only called if monkey ball is dead 
        private void BecomeNormalMonkey()
        {
            _ballGraphics.SetActive(false);
            transform.position = new Vector3(transform.position.x, -0.75f, transform.position.z);
            _graphics.transform.localPosition = Vector3.zero; 
            _checkSurroundings.BearerType = type = AIType.Monkey; 
            _agentHp.CurrentValue = 1; 
            _attackRange = 1f; // would have been better to store each mob General stats into a scriptable object so that I can assign the right values 
            wasMonkeyBall = true; 
            
            OnRequireStateChange(States.Attack, StateTransition.Safe); 
        }

        void Attack_Exit()
        {
            if (HasBeenInvokedByBoss)
            {
                TransitionToNewState(States.Attack, StateTransition.Safe); // debug crados             
            }

            switch (type)
            {
                case AIType.MonkeySurBall: 
                    break;
                case AIType.Fakir:
                    CancelInvoke(nameof(FakirAttack)); 
                    break; 
            }

            _hasCalledFakeCac = false;
            _exitingAttackState = true; // problematic to not have this on that kind of state machine (tradeoff for simplicity) 

            if (Type == AIType.Fakir && !_canPatrol) 
            { 
                _aIAnimation.PlayAnimation(AnimState.Idle, AnimDirection.Right);  
            }
            else
            {
                _agent.destination = _canPatrol ? _patrol.Points[_patrol.DestPoint].position : _idlePositionBeforeAttacking; // TODO : use closest point of list instead (when patrolling)
                _agent.speed = DefaultSpeed / _attackStateSpeedMultiplier;
            }
        }

        #endregion

        #region Defend

        IEnumerator Defend_Enter()
        {
            yield return new WaitForSeconds(_monkeyBallDodgeReactionTime);
            Debug.Log("defend_enter");

            _agent.speed = 0f;
            _checkSurroundings.CanDodgeProjectile = _monkeyBallCollider.enabled = _ballCollider.enabled = false;
            transform.position = _checkSurroundings.DodgeDirection * _monkeyBallDodgeDistance;  
            
            yield return new WaitForSeconds(0.1f);  
            _monkeyBallCollider.enabled = true;
            _ballCollider.enabled = true;
            _aIAnimation.PlayAnimation(AnimState.Miss, AnimDirection.None); 
            
            // MISS anim 
            
            yield return new WaitForSeconds(_monkeyBallProvocDuration); 
            OnRequireStateChange(States.Attack, StateTransition.Safe); 
            // risky if the player has gone outside of detection collider.. should I use a HFSM instead ? 
        } 

        void Defend_Exit()  
        { 
            _agent.speed = DefaultSpeed;
            _checkSurroundings.RightWallDetected = _checkSurroundings.LeftWallDetected = false; 
            _checkSurroundings.CanDodgeProjectile = true; 
        }

        #endregion

        #region Die
        IEnumerator Die_Enter() 
        {
            _patrol.IsDead = _checkSurroundings.IsDead = true; // DEBUG
            _agent.speed = 0f; 
            Debug.Log("die_enter");
            foreach (var item in _componentsToDeactivateOnDeath)
            {
                item.enabled = false; 
            } 
            
            yield return new WaitForSeconds(0.25f);  
            CancelInvoke();
            Clip clipToPlay = null;  
 
            try
            { 
                clipToPlay = _aIAnimation.PlayAnimation(wasMonkeyBall ? AnimState.Die : AnimState.Hit, AnimDirection.None);
            } 
            catch (Exception) { }

            if (clipToPlay != null) yield break;
            Debug.Log("Calling Die state instead of Hit state");
            _aIAnimation.PlayAnimation(AnimState.Die, AnimDirection.None); // need consistent naming across all mobs, not Die or Hit for same result.. 
        } 
        
        #endregion

#endregion
    }
}
