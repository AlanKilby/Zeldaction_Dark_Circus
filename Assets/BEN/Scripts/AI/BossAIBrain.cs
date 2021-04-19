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
    public float SpawnProbability { get => _spawnProbability; }
    public GameObject Prefab { get => _prefab; } 
}

// temporary all in one solution
// will upgrade to a Behaviour Tree very soon  
public class BossAIBrain : MonoBehaviour
{
    // [SerializeField] private GameObject _graphics;
    [Header("Spawn")]
    [SerializeField, Space] private GameObject _spawner; // upgrade to have more creative control and use less prefabs 
    [SerializeField] private List<SpawnableEntity> _spawnableEntitiesList; // use a HashSet instead to avoid duplicates 
    [SerializeField, Range(5, 30)] private float invocationDelay = 15f;
    private List<Vector3> _spawnPositions = new List<Vector3>(); 
     
    [Header("Patterns")]
    public bool attackPatterns;
    public bool spawnPatterns;
    public bool lightGlowPatterns; 

    private StateMachine<States> _fsm;
    private EditorCoroutine _editorCoroutine;
    private AIAnimation _aIAnimation;

    public static Action<States, StateTransition> OnRequireStateChange;

    private float m_invocationSelector; 
    private int m_entityToInvokeSelector;

    private bool m_canInvoke;
    private bool _lightsAreOff; 

    [Header("DEBUG")]
    public bool invokeOnStart; 

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
        for (int i = 0; i < _spawner.transform.childCount; i++)
        {
            _spawnPositions.Add(_spawner.transform.GetChild(i).position);
        } 

        if (invokeOnStart)
        {
            InvokeEntity(1f); 
        }

        StartCoroutine(SetInvocationCooldown(invocationDelay));
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

        _fsm.ChangeState(States.Attack, StateTransition.Safe);
    }

    void Init_Exit()
    {
        Debug.Log("Transition to default state");
    }
#endregion

    void Attack_Enter()
    {

    }

#region Attack
    void Attack_FixedUpdate()
    {
        if (m_canInvoke)
        {
            InvokeEntity(1f); 
        }
    }

    void Attack_Exit()
    {

    }
#endregion

#region Defend
    void Defend_Enter()
    {
        // when lights are off
    }

    void Defend_FixedUpdate()
    {

    }

    void Defend_Exit() 
    {
        // when lights are out 
    }
    #endregion

    #endregion

    #region Functionality
    private void InvokeEntity(float invocationProbability)
    {
        // try invoking for each spawn point
        for (int i = 0; i < _spawner.transform.childCount; i++)  
        {
            // try invoking
            m_invocationSelector = UnityEngine.Random.Range(0f, 1f);

            if (m_invocationSelector <= invocationProbability)
            {
                // if invocation, select entity
                m_entityToInvokeSelector = UnityEngine.Random.Range(0, _spawnableEntitiesList.Count);
                GameObject instanceReference = Instantiate(_spawnableEntitiesList[i].Prefab, _spawnPositions[i], Quaternion.identity); 
                instanceReference.GetComponent<BasicAIBrain>().HasBeenInvokedByBoss = true;  
            }
        }

        // why not increase invocation probability if it failed 
        StartCoroutine(SetInvocationCooldown(invocationDelay));
    }

    IEnumerator SetInvocationCooldown(float delay)
    {
        m_canInvoke = false; 

        yield return new WaitForSeconds(delay);
        m_canInvoke = true;
    }
    #endregion
}
