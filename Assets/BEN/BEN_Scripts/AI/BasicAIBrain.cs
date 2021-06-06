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
using UnityEngine.Events;


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
    [DefaultExecutionOrder(2)] 
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
        [SerializeField, Range(0f, 5f)] private float _delayBetweenEachAttack = 1f; 
        [Space, SerializeField, ConditionalShow("isMonkeyBall"), Tooltip("Delay before jumping back to ball again")] private float _monkeyBallProvocDuration = 3f;
        [SerializeField, ConditionalShow("isMonkeyBall")] private float _monkeyBallDodgeReactionTime = 0.5f;
        [SerializeField, ConditionalShow("isMonkeyBall"), Tooltip("how far monkey ball goes when dodging")] private float _monkeyBallDodgeDistance = 5f;

        [Header("Other")] 
        [Space, SerializeField] private GameObject _graphics; // MOVE TO AIANIMATION
        [SerializeField] private GameObject _detection;
        [SerializeField] private GameObject _shadow;
        [SerializeField] private PlaceholderDestination _placeholderDestination;
        [SerializeField, ConditionalShow("isMonkeyBall", true)] private GameObject _ballGraphics;
        [SerializeField] private Behaviour[] _componentsToDeactivateOnDeath;
        [SerializeField, Range(0f, 5f)] private float _graphicsDisableDelay = 0f; 

        [Header("-- DEBUG --")]
        [SerializeField] private EditorDebuggerSO _debugger;
        [SerializeField] private bool refresh;
        private bool wasMonkeyBall;

        // ALAN Variables
        //public UnityEvent playerHitEvent;
        
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
    private int _currentParentRotation; // MOVE TO AIANIMATION
        
    private Health _playerHP;
    private bool _exitingAttackState;

    private Collider _monkeyBallCollider;
    private Collider _ballCollider; 
    private bool _hasAppliedCACDamage;
    private Timer _timer;
    private static readonly int LookingRight = Animator.StringToHash("lookingRight");

    #endregion

#region Properties

        public float DelayBeforeBackToDefaultState { get ; private set ; } 
        public float DefaultSpeed { get; set; }
        public float InitialSpeed { get; private set; }
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
            _agentHp.IsMonkeyBall = true; 
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
                }
            }
            catch (Exception e) 
            {
                // Debug.Log($"{ e.Message} thrown by {gameObject.name}"); 
                _patrol = GetComponent<FsmPatrol>();
                _patrol.SetPoints();
            } 
        }

        private void Start()
        {
            _playerHP = PlayerMovement_Alan.sPlayer.GetComponentInChildren<Health>();
            DelayBeforeBackToDefaultState = _delayBeforeBackToDefaultState;
            GoingBackToPositionBeforeIdling = false;
            DefaultSpeed = InitialSpeed = _defaultSpeed; 
            MonkeyBallDodgeDistance = _monkeyBallDodgeDistance;

            if (type == AIType.MonkeySurBall)
            {
                _ballAnimation = _ballGraphics.GetComponent<AIAnimation>(); 
            }

            _patrol = GetComponent<FsmPatrol>();

            if (HasBeenInvokedByBoss || !_canPatrol)
            {
                _patrol.enabled = false; 
                _agentHp.CurrentValue = 1; // will be overwritten by health. Just to avoid 0 hp when invoked by boss
            }
            else 
            {
                _patrol.SetPoints();  
            }

            _agent = GetComponent<NavMeshAgent>();
            _checkSurroundings = GetComponentInChildren<CheckSurroundings>();

            if (Type == AIType.MonkeySurBall)
            {
                _monkeyBallCollider = GetComponent<BoxCollider>(); 
                _ballCollider = GetComponentInChildren<SphereCollider>();
            }

            _agent.speed = DefaultSpeed;
            _currentParentRotation = -1; // modif 04.06 
        } 

        private void FixedUpdate()
        {
            if (_canPatrol && !HasBeenInvokedByBoss)
            {
                _detection.transform.rotation = Quaternion.Euler(0f, _placeholderDestination.EulerAnglesY, 0f);
            } 
            CheckAnimDirection(); // remove from state machine 

            if (_agentHp.CurrentValue <= 0 && !_patrol.IsDead && Type != AIType.MonkeySurBall) 
            {
                // Debug.Log("transition to death state"); 
                OnRequireStateChange(States.Die, StateTransition.Safe); 
            }

            if (!_canPatrol && Vector3.Distance(transform.position, _idlePositionBeforeAttacking) <= 0.25f && _exitingAttackState) 
            { 
                _exitingAttackState = false; 
                _agent.speed = 0f;
                _aIAnimation.PlayAnimation(AnimState.Idle, AnimDirection.Right); // use AnimDirection according to where you come from . 
            } 
            
            if (!PlayerMovement_Alan.sPlayer)
            {
                OnRequireStateChange(States.Default, StateTransition.Safe); 
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
            if (Type == AIType.Fakir && !_canPatrol) return;  

            _animDirection = (AnimDirection) (_placeholderDestination.angleIndex); 

            if (_placeholderDestination.angleIndex == _currentParentRotation) return;
            
            _aIAnimation.PlayAnimation(AnimState.Walk, _animDirection);

            if (type == AIType.MonkeySurBall)
            {
                _ballAnimation.PlayAnimation(AnimState.Walk, _animDirection); 
            }
            
            _currentParentRotation = _placeholderDestination.angleIndex;
        } 
        
        private void CheckAnimDirection(AnimState state)
        {
            if (Type == AIType.Fakir && !_canPatrol) return;  
            
            _animDirection = (AnimDirection) _placeholderDestination.angleIndex;

            if (_placeholderDestination.angleIndex == _currentParentRotation) return; 
            
            _aIAnimation.PlayAnimation(state, _animDirection);
            
            if (type == AIType.MonkeySurBall)
            {
                _ballAnimation.PlayAnimation(AnimState.Walk, _animDirection); 
            }
            
            _currentParentRotation = _placeholderDestination.angleIndex;
        } 
        
        // <===
        
        #region FSM

        #region Init 

        void Init_Enter()
        {
            _aIAnimation = _graphics.GetComponent<AIAnimation>();
            _fsm.ChangeState(NewState = States.Default, StateTransition.Safe);
            // Debug.Log("init_enter");

            NewState = States.Init;
        }

        void Init_Exit()
        {
        }
        
        #endregion  
        
        #region Default
        IEnumerator Default_Enter()  
        { 
            yield return new WaitForSeconds(0.03f);
            NewState = States.Default;

            if ((_canPatrol || GoingBackToPositionBeforeIdling) && !HasBeenInvokedByBoss)
            {
                if (Type == AIType.Fakir && !_canPatrol)
                {
                    _aIAnimation.PlayAnimation(AnimState.Idle, AnimDirection.Right); 
                }
                else
                { 
                    _aIAnimation.PlayAnimation(AnimState.Walk, _animDirection);

                    if (type == AIType.MonkeySurBall)
                    {
                        _ballAnimation.PlayAnimation(AnimState.Walk, _animDirection);
                    } 
                    
                    CheckAnimDirection(AnimState.Walk);
                }
            } 
            else 
            { 
                _aIAnimation.PlayAnimation(AnimState.Idle, AnimDirection.Right);
                
                if (type == AIType.MonkeySurBall)
                {
                    var clip = _ballAnimation.PlayAnimation(AnimState.Idle, AnimDirection.None); 
                } 
            } 
        } 

        private void Default_Exit() { }

        #endregion 

        #region Attack
        
        private IEnumerator Attack_Enter() // UPGRADE : use async-await coroutines
        {
            yield return new WaitForSeconds(_attackDelay);
            NewState = States.Attack;
            
            _agent.destination = TargetToAttackPosition;
            _idlePositionBeforeAttacking = transform.position;

            var clip = new Clip();
            
            switch (type) 
            {
                case AIType.Monkey:
                    clip = _aIAnimation.PlayAnimation(wasMonkeyBall ? AnimState.SecondaryAtk : AnimState.Atk, _animDirection);
                    break; 
                case AIType.MonkeySurBall:
                    clip = _aIAnimation.PlayAnimation(AnimState.Atk, _animDirection); 
                    break;
                case AIType.Mascotte:
                    clip = _aIAnimation.PlayAnimation(AnimState.Walk, _animDirection);
                    break;
                case AIType.Fakir:
                    _agent.speed = 0f;
                    clip = _aIAnimation.PlayAnimation(AnimState.Atk, _animDirection);
                    InvokeRepeating(nameof(FakirAttack), 0f, _attackRate);  
                    break; 
            } 
            
            _timer.SetTargetValue(_delayBetweenEachAttack + clip.clipContainer.length); 
        } 

        private void Attack_FixedUpdate()  
        {
            if (Vector3.Distance(transform.position, PlayerMovement_Alan.sPlayerPos) <= _attackRange) 
            {
                _agent.speed = 0f;

                if (Type == AIType.Fakir || type == AIType.MonkeySurBall) // why checking if new state is attack ??? 
                {
                    var (targetIsReached, isFirstFrame) = _timer.Update(); 
                    CheckAnimDirection(AnimState.Atk); 
                    
                    if (isFirstFrame)  
                    {
                        _aIAnimation.PlayAnimation(AnimState.Atk, _animDirection);
                    }
                    else if (targetIsReached) 
                    {
                        _timer.Reset();  
                    } 

                    if (type == AIType.MonkeySurBall)
                    {
                        _aIAnimation.animator.SetBool(LookingRight, _animDirection == AnimDirection.Right);
                    }
                } 

                // back to default when player has been killed
                if (LoadSceneOnPlayerDeath.sPlayerIsDead)
                {
                    _fsm.ChangeState(States.Default, StateTransition.Safe);
                    CancelInvoke(nameof(ApplyCACDamage)); 
                }

                // to simulate player killed from CAC. Distance is done from projectile
                if ((type != AIType.Monkey && type != AIType.Mascotte) || _hasAppliedCACDamage) return;
                _hasAppliedCACDamage = true;

                if (type == AIType.Mascotte)
                {
                    _aIAnimation.PlayAnimation(AnimState.Atk, _animDirection);  
                }
                 
                InvokeRepeating(nameof(ApplyCACDamage), 0.5f, _attackRate);
            }
            else 
            {
                _agent.speed = DefaultSpeed * _attackStateSpeedMultiplier; 
                _agent.destination = PlayerMovement_Alan.sPlayerPos;
                _timer.Reset();  

                CancelInvoke(nameof(ApplyCACDamage));
                _hasAppliedCACDamage = false;

                _aIAnimation.PlayAnimation(wasMonkeyBall ? AnimState.SecondaryAtk : // if wasMonkeyBall THEN type == AIType.Monkey 
                                            type == AIType.Monkey ? 
                                                AnimState.Atk : 
                                                AnimState.Walk, _animDirection); 
                
                CheckAnimDirection(type == AIType.Monkey ? AnimState.Atk : AnimState.Walk);
            }
        } 

        private void ApplyCACDamage()
        {
            if(!AK_PlayerHit.isInvincible)
            _playerHP.DecreaseHp(_attackDamage);

            // Added by Alan 06/06/2021
            PlayerMovement_Alan playerMov = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement_Alan>();
            playerMov.HitAnim();
            // Added by Alan 03/06/2021
            //playerHitEvent.Invoke();
        } 

        private void FakirAttack() 
        {
            _fakirProjectile.GetComponentInChildren<ParabolicFunction>().CasterTransform = _graphics.transform;
        }

        // only called if monkey ball is dead 
        private void BecomeNormalMonkey()
        {
            Destroy(_ballGraphics); 
            transform.position = new Vector3(transform.position.x, -0.75f, transform.position.z);
            _graphics.transform.localPosition = Vector3.zero; 
            _checkSurroundings.BearerType = type = AIType.Monkey; 
            _agentHp.CurrentValue = 1; 
            _attackRange = 2f; // would have been better to store each mob General stats into a scriptable object so that I can assign the right values 
            wasMonkeyBall = true;
            _agent.destination = PlayerMovement_Alan.sPlayerPos;
            _attackDelay = 0.2f; 
            
            OnRequireStateChange(States.Attack, StateTransition.Safe); 
        }

        void Attack_Exit()
        {
            if (HasBeenInvokedByBoss)
            {
                TransitionToNewState(States.Attack, StateTransition.Safe);  
            }

            switch (type)
            {
                case AIType.MonkeySurBall: 
                    break;
                case AIType.Fakir:
                    CancelInvoke(nameof(FakirAttack)); 
                    break; 
            }

            _hasAppliedCACDamage = false;
            _exitingAttackState = true; // problematic to not have this on that kind of state machine (tradeoff for simplicity) 

            if (Type == AIType.Fakir && !_canPatrol) 
            { 
                _aIAnimation.PlayAnimation(AnimState.Idle, AnimDirection.Right);  
            }
            else
            {
                if (HasBeenInvokedByBoss) return;
                _agent.destination = _canPatrol ? _patrol.Points[_patrol.DestPoint].position : _idlePositionBeforeAttacking; // A FAIRE : use closest point of list instead (when patrolling)
                _agent.speed = DefaultSpeed / _attackStateSpeedMultiplier; 
            }
        }

        #endregion 

        #region Defend

        IEnumerator Defend_Enter()
        {
            _agent.speed = 0f;
            NewState = States.Defend; 

            switch (Type)
            {
                case AIType.MonkeySurBall:
                    yield return new WaitForSeconds(_monkeyBallDodgeReactionTime);
                    _checkSurroundings.CanDodgeProjectile = _monkeyBallCollider.enabled = _ballCollider.enabled = false;
                    transform.position += _checkSurroundings.DodgeDirection * _monkeyBallDodgeDistance;  
            
                    yield return new WaitForSeconds(0.1f);  
                    _monkeyBallCollider.enabled = _ballCollider.enabled = true;
                    _aIAnimation.PlayAnimation(AnimState.Miss, AnimDirection.None); 
                    
                    yield return new WaitForSeconds(_monkeyBallProvocDuration); 
                    OnRequireStateChange(States.Attack, StateTransition.Safe);  
                    break;
                case AIType.Mascotte:
                    _aIAnimation.PlayAnimation(AnimState.Hit, _animDirection);
                    
                    yield return new WaitForSeconds(1f);  
                    OnRequireStateChange(States.Attack, StateTransition.Safe); // would have been better to use HFSM 
                    break;
            }
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
            NewState = States.Die;
            _patrol.IsDead = _checkSurroundings.IsDead = true; // DEBUG
            _agent.speed = 0f; 
            foreach (var item in _componentsToDeactivateOnDeath)
            {
                item.enabled = false; 
            }

            StartCoroutine(nameof(DisableGraphics)); // so that I don't hold the whole Die_Enter coroutine for _graphicsDisableDelay seconds
            
            yield return new WaitForSeconds(0.25f);  
            CancelInvoke();
            Clip clipToPlay = null;  
 
            try
            { 
                clipToPlay = _aIAnimation.PlayAnimation(wasMonkeyBall ? AnimState.Die : AnimState.Hit, AnimDirection.None);
            } 
            catch (Exception) { }

            if (clipToPlay != null) yield break;
            _aIAnimation.PlayAnimation(AnimState.Die, AnimDirection.None); // need consistent naming across all mobs, not Die or Hit for same result.. 
        }

        IEnumerator DisableGraphics()
        {
            yield return new WaitForSeconds(_graphicsDisableDelay); 
            _graphics.GetComponent<SpriteRenderer>().enabled = false; 
            _shadow.SetActive(false);
        }
        
        #endregion

#endregion
    }
}
