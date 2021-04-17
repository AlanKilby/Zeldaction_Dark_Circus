using System;
using System.Collections;
using UnityEngine;
using MonsterLove.StateMachine;
using Debug = UnityEngine.Debug;
using Unity.EditorCoroutines.Editor;
using UnityEngine.AI;

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
        Clear = 8 
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
        [SerializeField, Tooltip("DefaultSpeed increse when rushing toward the player. 1 = no increase"), Range(1f, 3f)] private float attackStateSpeedMultiplier = 1.25f;
        [SerializeField, Tooltip("Delay from Idle to Attack State when player is detected"), Range(0f, 5f)] private float attackDelay = 1f; 
        [SerializeField, Range(3f, 10f)] private float distanceRange = 8f;
        [SerializeField, Range(1f, 3f)] private float cacRange = 1.5f; 
        private StateMachine<States> _fsm; 

        public static Func<Transform[]> OnQueryingChildPosition;
        private bool _bPatrolZoneIsSet;
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

        #region Editor

        private void OnValidate()
        {
            if (Application.isPlaying) return;

            isMonkeyBall = false;
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

            if (_ball && Type != AIType.Monkey) 
            {
                UnityEngine.Object[] objectsToDestroy = new UnityEngine.Object[] {_ball };
                EditorCoroutineUtility.StartCoroutine(DestroyImmediate(objectsToDestroy), this);
            }
            else if (_patrolZone && (Type == AIType.Undefined || Type == AIType.Ball))
            {
                UnityEngine.Object[] objectsToDestroy = new UnityEngine.Object[] { _patrolZone, _patrol };
                EditorCoroutineUtility.StartCoroutine(DestroyImmediate(objectsToDestroy), this);
            }

            // create sibling and add script
            if (_patrolZone || Type == AIType.Ball) return; // WARNING : logic flaw with precedent block

            _patrolZone = new GameObject(); 
            _patrolZone.transform.SetParent(transform.parent);
            _patrolZone.transform.SetAsLastSibling();
            _patrolZone.name = "PatrolZone";

            _patrol = gameObject.AddComponent<FsmPatrol>();
            _patrol.points = new Transform[2];

            _bPatrolZoneIsSet = true;

            // add default amount of children
            for (var i = 0; i < 2; i++)
            {
                var goChild = new GameObject();
                goChild.transform.SetParent(_patrolZone.transform);
                goChild.name = $"patrolPoint_{i}";
                goChild.transform.localPosition = new Vector3(0f, 1f, 0f + i);

                // populate fsm array 
                _patrol.points[i] = goChild.transform;
                Debug.Log("Creating fsmPatrol with default points set to two. Ctrl+D a patrolPoint and add it to the list if you want a longer path");
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

        private IEnumerator DestroyImmediate(UnityEngine.Object[] objectsToDestroy) 
        {
            yield return new EditorWaitForSeconds(0.02f); 
            foreach (var item in objectsToDestroy) 
            {
                DestroyImmediate(item, false); 
            } 
        }
        
        
#endregion

#region Unity Callbacks

        private void OnEnable()
        {
            OnRequireStateChange += TransitionToNewState;
        } 

        private void OnDisable()
        {
            OnRequireStateChange -= TransitionToNewState; 
        }

        void Awake()
        {
            _fsm = StateMachine<States>.Initialize(this); 
            _fsm.ChangeState(States.Init, StateTransition.Safe);
        }
 
        private void Start()
        {
            _patrol = GetComponent<FsmPatrol>();
            _agent = GetComponent<NavMeshAgent>();

            _agent.speed = defaultSpeed;
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

            switch (type)
            {
                case AIType.Monkey:
                    _aIAnimation.PlayAnimation(AnimationState.IdleRight); // make it dynamic direction instead 
                    break;
                case AIType.MonkeyBall:
                    /* var scriptableAnimation = GameManager.Instance.scriptableAnimationList[0];
                    _aIAnimation.SetScriptable(scriptableAnimation); */
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
                    _aIAnimation.PlayAnimation(AnimationState.AtkRight); // invert 
                    _ballAnimation.PlayAnimation(_moveRight); // hardcoded 
                    break;
                case AIType.Mascotte:
                    Debug.Log("Mascotte => attacking");
                    _aIAnimation.PlayAnimation(AnimationState.AtkLeft); // invert 
                    break;
                case AIType.Fakir:
                    Debug.Log("Fakir => attacking");
                    _aIAnimation.PlayAnimation(AnimationState.AtkRight); // invert 
                    break;
                default:
                    Debug.Log("Default => breaking");
                    break;
            }


            // abstracted away => this should be standard // make it dynamic direction instead 
        } 

        void Attack_FixedUpdate()  
        {
            if (Vector3.Distance(transform.position, PlayerMovement_Alan.sPlayerPos) <= 1.5f) // <= hardcodé
            {
                _agent.destination = transform.position; // risky floating-point error; 
                return;
            } // archi crade façon d'arrêter

            _agent.destination = PlayerMovement_Alan.sPlayerPos; 
            Debug.Log("Attacking fixedUpdate");
        } 

        void Attack_Exit()
        {
            Debug.Log("Attacking exit");
            _agent.destination = _positionBeforeAttacking;
            _agent.speed /= attackStateSpeedMultiplier; 
            _aIAnimation.PlayAnimation(AnimationState.IdleRight); // make it dynamic direction instead 
            // _ballAnimation.StopAnimating(); only when initial position is reached
        } 
        
#endregion        

#endregion
    }
}
