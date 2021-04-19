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
    [SerializeField] private AIType _type;
    [SerializeField] private GameObject _prefab;
    [SerializeField, Range(0f, 100f)] private float _spawnProbability = 30f;
    public AIType Type { get => _type; } 
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
    [SerializeField, Range(5, 60)] private float invocationDelay = 20f;
    private List<Vector3> _spawnPositions = new List<Vector3>(); 
     
    [Header("Patterns")]
    public bool attackPatterns;
    public bool spawnPatterns;
    public bool lightGlowPatterns; 
     
    private StateMachine<States> _fsm;
    private EditorCoroutine _editorCoroutine;
    private AIAnimation _aIAnimation;
    [SerializeField] private List<Switch> _switchedList = new List<Switch>();
    [SerializeField] private byte maxActiveSwitches = 2;
    private byte activeSwtiches; 

    public static Action<States, StateTransition> OnRequireStateChange;

    private float m_invocationSelector; 
    private int m_entityToInvokeSelector;

    private bool m_canInvoke = true;
    private bool _canAttack = true; 
    private bool _lightsAreOff; 

    [Header("DEBUG")]
    public bool invokeOnStart; 
    [SerializeField] private Transform rayPlaceholderManager;
    [SerializeField] private List<GameObject> rayPlaceholderVisuals;
    public bool debugRay;
    public bool doSpawns = true;

    public LayerMask playerLayer;
    private bool _showActivatableLigths = true;  // this should not be here 

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

        for (int i = 0; i < rayPlaceholderVisuals.Count; i++)
        {
            rayPlaceholderVisuals[i].SetActive(false);  
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
        // spawns and attack will sometimes overlap and the delay between one and the other will change over time (not same modulo) 

        if (m_canInvoke)
        {
            InvokeEntity(1f); 
        }

        if (_canAttack)
        {
            Attack();  
        }

        if (_showActivatableLigths)
        {
            for (int i = 0; i < _switchedList.Count && activeSwtiches < maxActiveSwitches; i++)
            {
                var selector = UnityEngine.Random.Range(0, 2);  
                if (selector > 0)
                {
                    _switchedList[i].ShowIsDeactivatable();
                    activeSwtiches++; 
                }
            }
            activeSwtiches = 0;
            StartCoroutine(SetSwitchesCooldown(15f)); 
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
        if (doSpawns)
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
                    GameObject instanceReference = Instantiate(_spawnableEntitiesList[m_entityToInvokeSelector].Prefab, _spawnPositions[i], Quaternion.identity);
                    BasicAIBrain basicAIBrain = instanceReference.GetComponent<BasicAIBrain>();
                    basicAIBrain.HasBeenInvokedByBoss = true;
                    basicAIBrain.TargetToAttackPosition = PlayerMovement_Alan.sPlayerPos;
                    BasicAIBrain.OnRequireStateChange(States.Attack, StateTransition.Overwrite); 
                    // basicAIBrain.Type = _spawnableEntitiesList[m_entityToInvokeSelector].Type; // warning risk of having basicAIBrain Type and type different 
                }
            }

            // why not increase invocation probability if it failed 
            StartCoroutine(SetInvocationCooldown(invocationDelay));
        }
    }

    private void Attack()
    {
        StartCoroutine(SetAttackCooldown(5f)); 

        rayPlaceholderManager.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 90f), 0f);
        List<Vector3> directions = new List<Vector3>();

        for (int i = 0; i < rayPlaceholderVisuals.Count; i++)  
        {
            if (!debugRay)
            {
                rayPlaceholderVisuals[i].SetActive(true); 
            }

            directions.Add(rayPlaceholderVisuals[i].transform.position);  
        }

        StartCoroutine(CastRayToPlayer(directions)); 
    }

    // possible : have a reference ray casting to the player and the others spreading from there 
    private IEnumerator CastRayToPlayer(List<Vector3> direction)
    {
        yield return new WaitForSeconds(2f); 
        Debug.Log("debugging ray cast"); 

        for (int i = 0; i < direction.Count; i++)
        {
            Debug.DrawLine(transform.position, (direction[i] - transform.position).normalized * 30f, Color.red, 2f, false);   
            if (Physics.Raycast(transform.position, (direction[i] - transform.position).normalized, out RaycastHit hitInfo, 30f,
                                     playerLayer, QueryTriggerInteraction.Collide))
            {
                Destroy(hitInfo.transform.gameObject);
                Debug.Log(hitInfo.transform.gameObject.layer); 
                Debug.Log("hitting player");
            } 
        }

        for (int i = 0; i < rayPlaceholderVisuals.Count; i++)
        {
            rayPlaceholderVisuals[i].SetActive(false); 
        }
    }

    IEnumerator SetInvocationCooldown(float delay)
    {
        m_canInvoke = false; 

        yield return new WaitForSeconds(delay);
        m_canInvoke = true;
    }

    // DRY
    IEnumerator SetAttackCooldown(float delay)
    {
        _canAttack = false;

        yield return new WaitForSeconds(delay); 
        _canAttack = true;
    }

    // SUPER DRY
    IEnumerator SetSwitchesCooldown(float delay)
    {
        _showActivatableLigths = false;

        yield return new WaitForSeconds(delay);
        _showActivatableLigths = true;
    }
    #endregion
}
