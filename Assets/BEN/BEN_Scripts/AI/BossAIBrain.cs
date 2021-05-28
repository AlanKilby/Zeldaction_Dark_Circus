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

public enum BossStates { Init, Default, Defend, Invocation, RayAttack, ObjectFalling, Death }
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

    private StateMachine<BossStates> _fsm;

    private AIAnimation _aIAnimation;
    private byte _activeSwitches; 

    public static Action<BossStates, StateTransition> OnRequireStateChange;

    private float _invocationSelector; 
    private float _entityToInvokeSelector;

    private bool _canInvoke = true; 
    private bool _canRayAttack = true; 
    private bool _canDoAirAttack = false;
    private bool _lightsAreOff;
    private BossStates currentState; 

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
        _fsm = StateMachine<BossStates>.Initialize(this);
        _fsm.ChangeState(BossStates.Init, StateTransition.Safe);
    }

    private void OnEnable()
    {
        OnRequireStateChange += TransitionToNewState; 
    }

    private void OnDisable()
    {
        OnRequireStateChange -= TransitionToNewState;
    }
    
    private void TransitionToNewState(BossStates newState, StateTransition transition)
    {
        currentState = newState; 
        _fsm.ChangeState(newState, transition);
    }

    void Start() 
    { 
        Debug.Log("start");
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
        
        _fsm.ChangeState(BossStates.RayAttack, StateTransition.Safe);
    }

    private void FixedUpdate()
    { 
        if (!PlayerMovement_Alan.sPlayer && Time.time >= 1f) // to avoid it on first frame
        {
            OnRequireStateChange(BossStates.Default, StateTransition.Safe); 
        }
        
        if (_canRayAttack) 
        {
            StartCoroutine(SetAttackCooldown(5f)); 
            Debug.Log("trying ray attack from FixedUpdate");
            OnRequireStateChange(BossStates.RayAttack, StateTransition.Safe); 
        } 
        else if (_canInvoke)
        {
            StartCoroutine(SetInvocationCooldown(_invocationDelay)); 
            Debug.Log("trying invocation from FixedUpdate");
            OnRequireStateChange(BossStates.Invocation, StateTransition.Safe); 
        }

        if (!_showActivatableLigths) return;
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

    private IEnumerator InvokeOnStart()  
    {
        yield return new WaitForSeconds(_invokeOnStartDelay); 
        Debug.Log("invoking entity on start");
        OnRequireStateChange(BossStates.Invocation, StateTransition.Safe); 
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
        Debug.Log("Initializing"); 
        currentState = BossStates.Init;
        // _aIAnimation = _graphics.GetComponent<AIAnimation>(); 
    }

    void Init_Exit()
    {
        
        
    }
    
#endregion 
    
#region Default

    void Default_Enter()
    {
        currentState = BossStates.Default;
        Debug.Log("default state");
    } 
    
#endregion

#region RayAttack

    void RayAttack_Enter()
    {
        Debug.Log("trying to attack");
        currentState = BossStates.RayAttack;
        SetAttackRotation();
    }

    void RayAttack_Exit()
    {
    }
    
#endregion

#region Invocation

    void Invocation_Enter()
    {
        Debug.Log("trying to invoke");
        currentState = BossStates.Invocation;
        InvokeEntity(1f); 
    } 

    void Invocation_Exit()
    { 
        
    }
    
#endregion

#region Object Falling

    void ObjectFalling_Enter()
    {
        currentState = BossStates.ObjectFalling;
    }

    void ObjectFalling_FixedUpdate()
    {
        
    }

    void ObjectFalling_Exit()
    {
        
    }

#endregion 

#region Defend 
    IEnumerator Defend_Enter()
    {
        // when lights are off
        currentState = BossStates.Defend;

        yield return new WaitForSeconds(_vulnerabilityDuration);
        TransitionToNewState(BossStates.RayAttack, StateTransition.Safe);  
    }

    void Defend_FixedUpdate()
    {
        Debug.Log("Ligth are off and boss is vulnerable"); 
    }

    void Defend_Exit() 
    {
        // when lights are on again => go back to RayAttack 
    }
    #endregion 
    
#region Death

     private void Death_Enter()
     {
         
     }
    
#endregion

#endregion
    
#region Functionality
    private void InvokeEntity(float invocationProbability)
    {
        if (!doSpawns) return;
        // StartCoroutine(SetInvocationCooldown(_invocationDelay));
        currentState = BossStates.Invocation;

        _isInvoking = true; // to avoid overlap of invocation and ray attack (too overwhelming, at least for the first phase)
        Debug.Log("invoking");

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
    }

    private void SetAttackRotation() 
    {
        // if (_isInvoking) return; 
        // StartCoroutine(SetAttackCooldown(5f)); 

        Debug.Log(" attacking");  

        _rayPlaceholderManager.rotation = Quaternion.Euler(0f, Random.Range(0f, 90f), 0f); // UPRGADE : less random, aim at player with some accuracy value
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

        StartCoroutine(CastRayToPlayer(directions)); 
    }

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
        _canInvoke = false;

        yield return new WaitForSeconds(delay);
        Debug.Log("setting can invoke to true"); 
        _canInvoke = true;
    } 

    // DRY
    IEnumerator SetAttackCooldown(float delay) 
    { 
        _canRayAttack = false;

        yield return new WaitForSeconds(delay); 
        Debug.Log("setting can attack to true");  
        _canRayAttack = true;
    } 
    
    // SUPER DRY
    IEnumerator SetCanDoAirAttack(float delay)  
    {
        Debug.Log("setting can do air attack to false"); 

        _canDoAirAttack = false;

        yield return new WaitForSeconds(delay); 
        _canDoAirAttack = true; 
    }

    // ...
    IEnumerator SetSwitchesCooldown(float delay)
    {
        _showActivatableLigths = false;

        yield return new WaitForSeconds(delay);
        _showActivatableLigths = true;
    }
    #endregion
}
