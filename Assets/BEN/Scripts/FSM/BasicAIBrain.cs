using System;
using System.Collections;
using System.Collections.Generic; 
using UnityEngine;
using MonsterLove.StateMachine;
using Debug = UnityEngine.Debug;
using Unity.EditorCoroutines.Editor;
using UnityEngine.AI;
#if UNITY_EDITOR
using UnityEditor;
#endif
 
/* 
 Very simple architecture (enum-based FSM) to avoid multiple classes and costly virtual calls
 
 All calls and processing are made from here with a switch based on AIType
 
 Upgrade to class-based FSM only if needed
 */

namespace BEN.Scripts.FSM 
{
    public enum AIType
    {
        Undefined = -1, 
        Monkey = 0, 
        MonkeyBall = 1,
        Ball = 2,
        Mascotte = 3,
        Fakir = 4  
    }
    
    public enum States
    {
        Init = 1, 
        Default = 2, 
        Attack = 4,
        Defend = 8,
        Clear = 16
    } 
    
    [RequireComponent(typeof(NavMeshAgent))]
    public class BasicAIBrain : MonoBehaviour
    {
        [SerializeField] private AIType type;
        public AIType Type => type; 
        
        // used for conditionalShow's property drawer until I know how to directly use enum 
        [HideInInspector] public bool isMonkeyBall;
         
        [SerializeField, ConditionalShow("isMonkeyBall", true)] private GameObject _monkeyBallProjectile;
        [SerializeField, Tooltip("Speed when patrolling"), Range(0.5f, 5f)] private float defaultSpeed = 2f;
        [SerializeField, Tooltip("Wait time between each attack"), Range(2f, 8f)] private float monkeyBallAttackRate = 4f;
        [SerializeField, Tooltip("Delay before jumping back to ball again"), Range(2f, 5f)] private float monkeyBallProvocDuration = 3f;
        [SerializeField, Tooltip("DefaultSpeed increse when rushing toward the player. 1 = no increase"), Range(1f, 3f)] private float attackStateSpeedMultiplier = 1.25f;
        [SerializeField, Tooltip("Delay from Idle to Attack State when player is detected"), Range(0f, 5f)] private float attackDelay = 1f; 
        [SerializeField, Range(3f, 10f)] private float distanceRange = 8f;
        [SerializeField, Range(1f, 3f)] private float cacRange = 1.5f;  
        private StateMachine<States> _fsm; 

        public static Func<Transform[]> OnQueryingChildPosition;
        private GameObject _patrolZone; 
        private GameObject _ball; 
        [SerializeField] private GameObject _graphics;
        
        private EditorCoroutine _editorCoroutine;
        private FsmPatrol _patrol;
        
        private NavMeshAgent _agent; 
        public bool playerDetected; // DEBUG

        public static Action<States, StateTransition> OnRequireStateChange;
        public Vector3 TargetToAttackPosition { get; set; }

        private AIAnimation _aIAnimation;
        private AIAnimation _ballAnimation; 

        private Vector3 _positionBeforeAttacking; // a node or single position 
        private Animator _ballAnimator = null; 
        private const int _moveRight = 1;

        private Sprite _sprite;
        private sbyte _destroying = -1;

        private Stack<FsmPatrol> _patrolStack = new Stack<FsmPatrol>();
        private CheckSurroundings _checkSurroundings; 

        [Header("-- DEBUG --")]
        [SerializeField] private EditorDebuggerSO debugger;

#region Editor

        private void OnValidate()
        {
            if (Application.isPlaying || _destroying != -1) return;

            isMonkeyBall = false;

            if (Type == AIType.MonkeyBall && !_ball)
            {
                try
                {
                    _ball = transform.GetChild(2).gameObject;
                }
                catch (Exception) { };
            }

            if (_ball)
            {
                debugger.BallAmount = 1; 
            }
            else 
            {
                debugger.BallAmount = 0; 
            }

            // trying to get reference after having it to null on destroy when playing with MonkeyBall
            // use Get/Set instead 

            if (Type != AIType.Undefined && Type != AIType.Ball)
            {
                try
                {
                    _graphics.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"{Type}_idle_resource");
                }
                catch (Exception e)
                {
                    _graphics = transform.GetChild(1).gameObject;
                    Debug.Log($"Catching error message {e.Message}"); 
                } 
            } 

            switch (type)
            { 
                case AIType.Monkey:
                    _graphics.transform.localPosition = Vector3.zero;
                    break;
                case AIType.MonkeyBall when !_ball:
                    if (debugger.BallAmount == 0)
                    {
                        _ball = new GameObject();
                        _ball.transform.SetParent(transform);
                        _ball.transform.SetAsLastSibling();
                        _ball.transform.localPosition = Vector3.zero;
                        _ball.name = "Ball_Graphics";
                        _ballAnimator = _ball.AddComponent<Animator>();
                        _ball.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("ball_resource"); // use static list in manager instead
                        _ball.AddComponent<SphereCollider>().isTrigger = true;
                        _ballAnimation = _ball.AddComponent<AIAnimation>();
                        _ballAnimation.SetType(AIType.Ball);
                        debugger.BallAmount++;
                    }

                    // _ball.AddComponent<NavMeshAgent>(); 
                    _graphics.transform.localPosition = Vector3.up; 
                    isMonkeyBall = true;
                    break;
                case AIType.Mascotte: 
                    _graphics.transform.localPosition = Vector3.up * 0.3f; 
                    break;
                case AIType.Fakir:
                    _graphics.transform.localPosition = Vector3.up * 0.25f;
                    break;
                default: 
                    _graphics.transform.localPosition = Vector3.zero;
                    _graphics.GetComponent<SpriteRenderer>().sprite = null; // WARNING : very unnefficient 
                    break; 
            } 

            if (debugger.BallAmount > 0 && Type != AIType.MonkeyBall) 
            {
                UnityEngine.Object[] objectsToDestroy = new UnityEngine.Object[] { _ball };
                EditorCoroutineUtility.StartCoroutine(DestroyImmediate(objectsToDestroy, 0.02f), this);
            }
            else if (_patrolZone && (Type == AIType.Undefined || Type == AIType.Ball))
            {
                UnityEngine.Object[] objectsToDestroy = new UnityEngine.Object[] { _patrolZone, _patrol };
                EditorCoroutineUtility.StartCoroutine(DestroyImmediate(objectsToDestroy, 0.02f), this);
                debugger.PatrolAmount--; 
            }

            // create sibling and add script
            if (_patrolZone || Type == AIType.Ball || Type == AIType.Undefined) return; // WARNING : logic flaw with precedent block

            _patrolZone = new GameObject(); 
            _patrolZone.transform.SetParent(transform.parent); 
            _patrolZone.transform.SetAsLastSibling();
            _patrolZone.name = "PatrolZone";

            _patrol = gameObject.AddComponent<FsmPatrol>();
            _patrol.points = new Transform[2];

            debugger.PatrolAmount++;

            if (debugger.PatrolAmount > 1)
            {
                UnityEngine.Object[] objectsToDestroy = new UnityEngine.Object[] { _patrolZone, _patrol };
                EditorCoroutineUtility.StartCoroutine(DestroyImmediate(objectsToDestroy, 0f), this);

                debugger.PatrolAmount = 1; 
            }

            // add default amount of children
            for (var i = 0; i < 2; i++)
            {
                var goChild = new GameObject();
                goChild.transform.SetParent(_patrolZone.transform);
                goChild.name = $"patrolPoint_{i}";
                goChild.transform.localPosition = new Vector3(0f, 1f, 0f + i);

                // populate fsm array 
                _patrol.points[i] = goChild.transform;
            }
        }

        private void OnDrawGizmos()
        { 
            if (!_patrol) return;

            for (var i = 0; i < _patrol.points.Length; i++)
            {
                if (i == 0)
                {
                    Gizmos.color = Color.yellow;
                }
                else
                {
                    Gizmos.color = Color.blue;
                }

                Gizmos.DrawWireSphere(_patrol.points[i].position, 0.25f);
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(_patrol.points[i].position, _patrol.points[(int)Mathf.Repeat(i + 1, _patrol.points.Length)].position);
            }
        }

        private IEnumerator DestroyImmediate(UnityEngine.Object[] objectsToDestroy, float delay) 
        {
            yield return new EditorWaitForSeconds(delay); 
            foreach (var item in objectsToDestroy) 
            {
                DestroyImmediate(item, false); 
            } 
        } 

        #endregion

        #region Unity Callbacks

        void Awake()
        {
            _fsm = StateMachine<States>.Initialize(this);
            _fsm.ChangeState(States.Init, StateTransition.Safe);
            _destroying = 0;
            Debug.Log("awake"); 
        }

        private void OnEnable()
        {
            OnRequireStateChange += TransitionToNewState;
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        } 

        private void Start()
        {
            _patrol = GetComponent<FsmPatrol>(); 
            _agent = GetComponent<NavMeshAgent>();
            _checkSurroundings = GetComponentInChildren<CheckSurroundings>(); 

            if (Type == AIType.MonkeyBall && !_ball)
            {
                try
                {
                    _ball = transform.GetChild(2).gameObject;
                }
                catch (Exception) { };
            }

            _agent.speed = defaultSpeed;
        }

        private void OnDisable()
        {
            OnRequireStateChange -= TransitionToNewState;
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
        } 

        private void OnDestroy()
        {
            _destroying = 1;
            Debug.Log("destroy"); 
        }
        
        public void OnBeforeAssemblyReload()
        {
            Debug.Log("Before Assembly Reload");
        } 

        public void OnAfterAssemblyReload()
        {
            Debug.Log("After Assembly Reload");
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
            _aIAnimation.SetType(type);

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

            switch (type)
            {
                case AIType.Monkey: 
                    _aIAnimation.PlayAnimation(AnimationState.WalkRight); // make it dynamic direction instead 
                    break;
                case AIType.MonkeyBall:
                    _aIAnimation.PlayAnimation(AnimationState.WalkRight); // make it dynamic direction instead
                    // _ballAnimation.PlayAnimation(); 
                    break;
                case AIType.Mascotte: 
                    _aIAnimation.PlayAnimation(AnimationState.WalkLeft); // make it dynamic direction instead  
                    break;
                case AIType.Fakir:
                    _aIAnimation.PlayAnimation(AnimationState.WalkRight); // make it dynamic direction instead  
                    break;
            } 
        } 

        void Default_FixedUpdate() 
        { 
            Debug.Log("updating default state");
            switch (type)
            {
                case AIType.Monkey:
                    break;
                case AIType.MonkeyBall:
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
            _positionBeforeAttacking = transform.position; 
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
            _agent.speed *= attackStateSpeedMultiplier;
            
            switch (type) 
            {
                case AIType.Monkey:
                    Debug.Log("Monkey => attacking");
                    _aIAnimation.PlayAnimation(AnimationState.AtkRight); 
                    break;
                case AIType.MonkeyBall:
                    Debug.Log("MonkeyBall => attacking");
                    _aIAnimation.PlayAnimation(AnimationState.AtkRight);
                    InvokeRepeating(nameof(MonkeyBallAttack), 0f, monkeyBallAttackRate); 
                    break;
                case AIType.Mascotte:
                    Debug.Log("Mascotte => attacking");
                    _aIAnimation.PlayAnimation(AnimationState.AtkLeft);
                    _graphics.transform.rotation = Quaternion.Euler(0f, 0f, 0f); 
                    break;
                case AIType.Fakir:
                    Debug.Log("Fakir => attacking");
                    _aIAnimation.PlayAnimation(AnimationState.AtkRight);  
                    break;
                default:
                    Debug.Log("Default => breaking");
                    break;
            }
            // abstracted away => this should be standard // make it dynamic direction instead 
        } 

        private void Attack_FixedUpdate()  
        {
            if (Vector3.Distance(transform.position, PlayerMovement_Alan.sPlayerPos) <= 1.5f) // <= hardcodé
            {
                _agent.speed = 0f; // risky floating-point error; 
            } // archi crade façon d'arrêter 
            else 
            {
                _agent.speed = defaultSpeed;
                _agent.destination = PlayerMovement_Alan.sPlayerPos;
            }

            Debug.Log("Attacking fixedUpdate");
        }

        private void MonkeyBallAttack()
        {
            GameObject reference = Instantiate(_monkeyBallProjectile, _graphics.transform.position, _graphics.transform.rotation);
            reference.transform.position = _graphics.transform.position;
            Debug.Log("INSTANTIATING"); 
        } 

        void Attack_Exit()
        {
            Debug.Log("Attacking exit");
            _agent.destination = _positionBeforeAttacking;
            _agent.speed /= attackStateSpeedMultiplier; 
            _aIAnimation.PlayAnimation(AnimationState.WalkRight); // make it dynamic direction instead 
                                                                  // _ballAnimation.StopAnimating(); only when initial position is reached

            switch (type)
            {
                case AIType.MonkeyBall: 
                    Debug.Log("MonkeyBall => exit attack");
                    _aIAnimation.PlayAnimation(AnimationState.WalkRight);
                    CancelInvoke(nameof(MonkeyBallAttack)); 
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
            _aIAnimation.PlayAnimation(11); // miss 
            _checkSurroundings.CanDodgeProjectile = false; 

            yield return new WaitForSeconds(monkeyBallProvocDuration); 
            OnRequireStateChange(States.Attack, StateTransition.Safe); 
        }

        void Defend_Exit() 
        { 
            Debug.Log("Defend_Exit");
            _agent.speed = defaultSpeed;
            _graphics.transform.localPosition = Vector3.up; 
            _ball.SetActive(true);
            _aIAnimation.PlayAnimation(AnimationState.AtkRight);
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
