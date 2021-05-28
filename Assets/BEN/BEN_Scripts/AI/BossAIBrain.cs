using BEN.AI;
using BEN.Animation;
using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

[Serializable]
public class SpawnableEntity
{
    [SerializeField] private AIType _type;
    [SerializeField] private GameObject _prefab;
    [SerializeField, Range(0f, 1f)] private float _spawnProbability = 0.5f; 
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
    [SerializeField] private bool _invokeOnStart;
    [SerializeField, Space] private GameObject _spawnerHalfCircle; // upgrade to have more creative control and use less prefabs
    [SerializeField, Space] private GameObject _spawnerOuter; // upgrade to have more creative control and use less prefabs 
    [SerializeField] private List<SpawnableEntity> _spawnableEntitiesList; // use a HashSet instead to avoid duplicates 
    [SerializeField, ConditionalShow("_invokeOnStart")] private float _invokeOnStartDelay = 10f; 
    [SerializeField, Range(5, 60)] private float _invocationDelay = 20f;
    private List<Vector3> _spawnPositionsHalfCircle = new List<Vector3>();
    private List<Vector3> _spawnPositionsOuter = new List<Vector3>();

    [Header("Attack")]
    public bool ray;
    public bool bombing; 

    [Header("Switches")]
    [SerializeField] private List<Switch> _switchedList = new List<Switch>();
    [SerializeField] private byte _maxActiveSwitches = 2;
    [SerializeField, Range(1, 15)] private float _vulnerabilityDuration = 5f;
    [SerializeField, Range(5, 50)] private float _percentOfDamageBeforeSwitchReset = 25f;
    public static float sBossVulnerabilityDuration; 

    private StateMachine<States> _fsm;

    private AIAnimation _aIAnimation;
    private byte _activeSwitches; 

    public static Action<States, StateTransition> OnRequireStateChange;

    private float _invocationSelector; 
    private float _entityToInvokeSelector;

    private bool _canInvoke = true; 
    private bool _canAttack = true; 
    private bool _lightsAreOff; 

    [Header("DEBUG")]
    [SerializeField] private Transform _rayPlaceholderManager;
    [SerializeField] private List<GameObject> _rayPlaceholderVisuals;
    public bool debugRay;
    public bool doSpawns = true;

    public LayerMask playerLayer;
    private bool _showActivatableLigths = true;  // this should not be here 
    public List<Transform> rayPlaceholderOrigin = new List<Transform>();
    private List<MeshRenderer> _rayMeshRenderers = new List<MeshRenderer>();
    private List<Collider> _rayColliders = new List<Collider>();
    private bool _isInvoking; 

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

    private void OnDisable()
    {
        OnRequireStateChange -= TransitionToNewState;
    }

    void Start()
    {
        _aIAnimation = GetComponentInChildren<AIAnimation>();
        sBossVulnerabilityDuration = _vulnerabilityDuration; 
        for (int i = 0; i < _spawnerHalfCircle.transform.childCount; i++)
        {
            _spawnPositionsHalfCircle.Add(_spawnerHalfCircle.transform.GetChild(i).position);
        }
        
        for (int i = 0; i < _spawnerOuter.transform.childCount; i++)
        {
            _spawnPositionsOuter.Add(_spawnerOuter.transform.GetChild(i).position);
        }

        for (int i = 0; i < _rayPlaceholderVisuals.Count; i++)
        {
            _rayMeshRenderers.Add(_rayPlaceholderVisuals[i].GetComponent<MeshRenderer>());
            _rayColliders.Add(_rayPlaceholderVisuals[i].GetComponent<Collider>());
            _rayColliders[i].enabled = false; 
        }

        if (_invokeOnStart)
        {
            StartCoroutine(nameof(InvokeOnStart)); 
        }
        else
        {
            StartCoroutine(SetInvocationCooldown(_invocationDelay));
        } 
    }

    private void FixedUpdate()
    {
        if (!PlayerMovement_Alan.sPlayer)
        {
            OnRequireStateChange(States.Default, StateTransition.Safe); 
        }
    }

    private IEnumerator InvokeOnStart()
    {
        yield return new WaitForSeconds(_invokeOnStartDelay);
        InvokeEntity(1f); 
    }

    private void TransitionToNewState(States newState, StateTransition transition)
    {
        _fsm.ChangeState(newState, transition);
    }

    private void SetLightOffState()
    {
        _lightsAreOff = true; 
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
    
#region Default
    
    void Default_Enter() { Debug.Log("default state");} 
    
#endregion

#region Attack 

    void Attack_FixedUpdate()
    {
        // spawns and attack will sometimes overlap and the delay between one and the other will change over time (not same modulo) 

        if (_canInvoke)
        {
            InvokeEntity(1f); 
        }

        if (_canAttack)
        {
            Attack();  
        }

        if (_showActivatableLigths)
        {
            for (int i = 0; i < _switchedList.Count && _activeSwitches < _maxActiveSwitches; i++)
            {
                var selector = UnityEngine.Random.Range(0, 2);  
                if (selector > 0)
                {
                    _switchedList[i].ShowIsDeactivatable();
                    _activeSwitches++; 
                }
            }
            _activeSwitches = 0;
            StartCoroutine(SetSwitchesCooldown(10f + _vulnerabilityDuration)); 
        }
    }

    void Attack_Exit()
    {

    }
    
#endregion

#region Defend
    IEnumerator Defend_Enter()
    {
        // when lights are off
        yield return new WaitForSeconds(_vulnerabilityDuration);
        TransitionToNewState(States.Attack, StateTransition.Safe);  
    }

    void Defend_FixedUpdate()
    {
        Debug.Log("Ligth are off and boss is vulnerable"); 
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
        if (!doSpawns) return;
        _isInvoking = true; // to avoid overlap of invocation and ray attack (too overwhelming, at least for the first phase)

        GameObject spawner = Random.Range(0, 2) == 0 ? _spawnerOuter : _spawnerHalfCircle;
        List<Vector3> spawnPositions = spawner == _spawnerOuter ? _spawnPositionsOuter : _spawnPositionsHalfCircle;
        
        // try invoking for each spawn point
        for (int i = 0; i < spawner.transform.childCount; i++)  
        {
            // try invoking
            _invocationSelector = Random.Range(0f, 1f);  

            if (_invocationSelector <= invocationProbability) 
            { 
                // if invocation, select entity
                _entityToInvokeSelector = UnityEngine.Random.Range(0f, 1f);
                var selector = _entityToInvokeSelector <= _spawnableEntitiesList[1].SpawnProbability ? 1 : 0; // super crados 
                
                GameObject instanceReference = Instantiate(_spawnableEntitiesList[selector].Prefab, spawnPositions[i], Quaternion.identity);
                BasicAIBrain basicAIBrain = instanceReference.GetComponentInChildren<BasicAIBrain>();
                basicAIBrain.HasBeenInvokedByBoss = true;
                basicAIBrain.TargetToAttackPosition = PlayerMovement_Alan.sPlayerPos;
                basicAIBrain.OnRequireStateChange(States.Attack, StateTransition.Overwrite); 
                // basicAIBrain.Type = _spawnableEntitiesList[m_entityToInvokeSelector].Type; // warning risk of having basicAIBrain Type and type different 
            }

            _isInvoking = false; 
        }

        // why not increase invocation probability if it failed 
        StartCoroutine(SetInvocationCooldown(_invocationDelay));
    }

    private void Attack()
    {
        StartCoroutine(SetAttackCooldown(5f));
        if (_isInvoking) return; 

        _rayPlaceholderManager.rotation = Quaternion.Euler(0f, Random.Range(0f, 90f), 0f);
        List<Vector3> directions = new List<Vector3>(); 

        for (int i = 0; i < _rayPlaceholderVisuals.Count; i++)  
        {
            if (debugRay)
            {
                _rayMeshRenderers[i].enabled = true;
                _rayColliders[i].enabled = false; 
            }

            directions.Add(_rayPlaceholderVisuals[i].transform.position - rayPlaceholderOrigin[i].position);   
        } 

        Debug.Log("attacking"); 
        StartCoroutine(CastRayToPlayer(directions)); 
    }

    // possible : have a reference ray casting to the player and the others spreading from there 
    private IEnumerator CastRayToPlayer(List<Vector3> direction)
    {
        yield return new WaitForSeconds(2f); 
        Debug.Log("debugging ray cast"); 

        for (var i = 0; i < _rayPlaceholderVisuals.Count; i++)
        { 
            _rayMeshRenderers[i].enabled = false; 
            _rayColliders[i].enabled = true; 
        }
    }

    IEnumerator SetInvocationCooldown(float delay)
    {
        Debug.Log("setting can invoke to false"); 

        _canInvoke = false; 

        yield return new WaitForSeconds(delay);
        _canInvoke = true;
    }

    // DRY
    IEnumerator SetAttackCooldown(float delay) 
    {
        Debug.Log("setting can attack to false"); 
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
