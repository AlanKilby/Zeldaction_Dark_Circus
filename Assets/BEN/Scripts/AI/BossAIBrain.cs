using BEN.Scripts; 
using BEN.Scripts.FSM;
using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;
using Debug = UnityEngine.Debug;
using Unity.EditorCoroutines.Editor; 
#if UNITY_EDITOR
using UnityEditor;
#endif
 
[System.Serializable]
public class SpawnableEntity
{
    [SerializeField] private string _name;
    [SerializeField] private GameObject _prefab;
    [SerializeField, Range(0f, 100f)] private float _spawnProbability = 30f;  
}

// temporary all in one solution
// will upgrade to a Behaviour Tree very soon
public class BossAIBrain : MonoBehaviour
{
    // [SerializeField] private GameObject _graphics;
    [Header("Spawn")]
    [SerializeField, Space] private List<Transform> _spawnPointsList;
    [SerializeField] private List<SpawnableEntity> _spawnableEntitiesList;
    [SerializeField, Range(5, 20)] private float invocationDelay = 10f;

    [Header("Patterns")]
    public bool attackPatterns;
    public bool spawnPatterns;
    public bool lightGlowPatterns; 

    private List<SpawnableEntity> _spawnablesList = new List<SpawnableEntity>();
    private StateMachine<States> _fsm;
    private EditorCoroutine _editorCoroutine;
    private AIAnimation _aIAnimation;

    public static Action<States, StateTransition> OnRequireStateChange;

    private float m_invocationSelector;
    private int m_platypusToInvokeSelector;

    private bool m_canInvoke;

    #region Unity Callbacks

    private void Awake()
    {
        _fsm = StateMachine<States>.Initialize(this);
        _fsm.ChangeState(States.Init, StateTransition.Safe);
        Debug.Log("awake");
    }

    private void OnEnable()
    {
        OnRequireStateChange += TransitionToNewState;
    }

    private void OnDisable()
    {
        OnRequireStateChange -= TransitionToNewState;
    }

    void Start()
    {
        _aIAnimation = GetComponentInChildren<AIAnimation>(); 
    } 

    private void TransitionToNewState(States newState, StateTransition transition)
    {
        _fsm.ChangeState(newState, transition);
    }

    #endregion

    #region FSM

    #region Init
    void Init_Enter()
    {
        Debug.Log("Initializing Default State");
        // _aIAnimation = _graphics.GetComponent<AIAnimation>(); 

        _fsm.ChangeState(States.Default, StateTransition.Safe);
    }

    void Init_Exit()
    {
        Debug.Log("Transition to default state");
    }
#endregion

#region Default
    void Default_Enter()
    {

    }

    void Default__FixedUpdate()
    {

    }

    void Default_Exit()
    {

    }
#endregion

    void Attack_Enter()
    {

    }

#region Attack
    void Attack_FixedUpdate()
    {

    }

    void Attack_Exit()
    {

    }
#endregion

#region Defend
    void Defend_Enter()
    {

    }

    void Defend_FixedUpdate()
    {

    }

    void Defend_Exit() 
    {

    }
    #endregion

    #endregion

    #region Functionality
    private void TryInvokePlatypus(float invocationProbability)
    {
        // try invoking for each spawn point
        for (int i = 0; i < _spawnableEntitiesList.Count; i++)
        {
            // try invoking
            m_invocationSelector = UnityEngine.Random.Range(0f, 1f);

            if (m_invocationSelector <= invocationProbability)
            {
                // if invocation, select entity
                m_platypusToInvokeSelector = UnityEngine.Random.Range(0, _spawnablesList.Count);
            }
        }

        // why not increase invocation probability if it failed 
        StartCoroutine(InvocationCooldown(invocationDelay));
    }

    IEnumerator InvocationCooldown(float delay)
    {
        m_canInvoke = false;

        yield return new WaitForSeconds(delay);
        m_canInvoke = true;
    }
    #endregion
}
