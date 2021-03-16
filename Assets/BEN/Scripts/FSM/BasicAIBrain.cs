using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using MonsterLove.StateMachine;
using Debug = UnityEngine.Debug;
using UnityEditor; 
using Unity.EditorCoroutines.Editor;

/* Currently supported methods are:

    Enter
    Exit
    FixedUpdate
    Update
    LateUpdate
    Finally
    
    It should be easy enough to extend the source to include other Unity Methods such as OnTriggerEnter, OnMouseDown etc
    
                                                                    === Transitions ===

There is simple support for managing asynchronous state changes with long enter or exit coroutines.

fsm.ChangeState(States.MyNextState, StateTransition.Safe);

The default is StateTransition.Safe. This will always allows the current state to finish both it's enter and exit functions before transitioning to any new states.

fsm.ChangeState(States.MyNextState, StateTransition.Overwrite);

StateMachine.Overwrite will cancel any current transitions, and call the next state immediately. This means any code which has yet to run in enter and exit routines will be skipped. If you need to ensure you end with a particular configuration, the finally function will always be called:

void MyCurrentState_Finally()
{
    //Reset object to desired configuration
}


This implementation uses reflection to automatically bind the state methods callbacks for each state. This saves you having to write endless boilerplate and generally makes life a lot more pleasant. But of course reflection is slow, so we try minimize this by only doing it once during the call to Initialize.

For most objects this won't be a problem, but note that if you are spawning many objects during game play it might pay to make use of an object pool, and initialize objects on start up instead. (This is generally good practice anyway).
Manual Initialization

In performance critical situations (e.g. thousands of instances) you can optimize initialization further but manually configuring the StateMachineRunner component. You will need to manually add this to a GameObject and then call:

StateMachines<States> fsm = GetComponent<StateMachineRunner>().Initialize<States>(componentReference);
 */

namespace BEN.Scripts.FSM 
{
    public enum AIType
    {
        Undefined = 0, 
        Monkey = 1, 
        MonkeyBall = 2, 
        SwordSpitter = 4, 
        Mascotte = 8
    }
    
    public enum States
    {
        Init = 1, 
        Default = 2, 
        Attack = 4,
        Clear = 8 
    }
    
    
    public class BasicAIBrain : MonoBehaviour
    {
        [SerializeField] private AIType type;
        [SerializeField, Range(0f, 5f)] private float attackDelay = 1f; 
        private StateMachine<States> _fsm;

        public static Func<Transform[]> OnQueryingChildPosition;
        private bool patrolZoneIsSet;
        private GameObject patrolZone;
        private EditorCoroutine _editorCoroutine;
        private FsmPatrol _patrol; 

        private void OnValidate()
        {
            // add a patrol zone when needed
            if (type != AIType.SwordSpitter && type != AIType.Mascotte)
            {
                if (patrolZoneIsSet) 
                {
                    EditorCoroutineUtility.StartCoroutine(DestroyImmediate(), this); 
                } 
                patrolZoneIsSet = false; 
                return; 
            }

            _patrol = gameObject.AddComponent<FsmPatrol>();  
            
            patrolZone = new GameObject();
            patrolZone.transform.SetParent(transform.parent);
            patrolZone.transform.SetAsLastSibling(); 
            patrolZone.name = "PatrolZone";
            
            for (var i = 0; i < 2; i++) 
            {
                var goChild = new GameObject(); 
                goChild.transform.SetParent(patrolZone.transform);
                goChild.name = $"patrolPoint_{i}";  
            }

            patrolZoneIsSet = true; 

        }
        
        private IEnumerator DestroyImmediate() 
        {
            yield return new EditorWaitForSeconds(0.02f); 
            DestroyImmediate(patrolZone, false);
            DestroyImmediate(_patrol, false);
        }

        void Awake()
        {
            _fsm = StateMachine<States>.Initialize(this); 
            _fsm.ChangeState(States.Init, StateTransition.Safe); // example
        }

        private void FixedUpdate()
        {
            if (Input.GetKeyDown(KeyCode.N)) // if player is detected
            {
                _fsm.ChangeState(States.Attack, StateTransition.Overwrite);
            }
        }

        void Init_Enter()
        {
            Debug.Log("Initializing Default State"); 
            
            // call this only if this NPC needs patrolling behaviour 
            _fsm.ChangeState(States.Default, StateTransition.Safe);
        }

        void Init_Exit()
        {
            Debug.Log("Transition to patrol state");
        }

        //Coroutines are supported, simply return IEnumerator
        // UPGRADE : use async/await coroutines instead
        private IEnumerator Attack_Enter()
        {
            yield return new WaitForSeconds(attackDelay); 
            Debug.Log($"Attacking in {attackDelay} seconds");
        }
        

        void Attack_FixedUpdate()
        {
            Debug.Log("Attacking fixedUpdate");
        }

        void Attack_Exit()
        {
            Debug.Log("Attacking exit");
        }

        void Default_Enter()
        { 
            Debug.Log("entering patrol state");
            switch (type)
            {
                case AIType.MonkeyBall:
                    var patrol = gameObject.AddComponent<AgentPatrol>();
                    patrol.points = OnQueryingChildPosition(); 
                    break; 
            }
        }

        void Default_FixedUpdate()
        {
            
        }
        
        void Default_Finally()
        {
            //Reset object to desired configuration
        }
    }
}
