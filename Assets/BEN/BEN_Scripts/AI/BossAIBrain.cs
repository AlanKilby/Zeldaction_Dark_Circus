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

public enum BossStates { Init, Default, Vulnerable, Invocation, ObjectFalling, Death } 
public enum SwitchesPattern { LineOne, LineTwo, LineThree, DiagonalOne, DiagonalTwo, FullRight, FullLeft } 
public class BossAIBrain : MonoBehaviour
{
    // [SerializeField] private GameObject _graphics;
    [SerializeField] private Collider _bossCollider;
    [SerializeField] private Health _bossHP;


    [Header("Spawn")]
    [SerializeField, Space] private GameObject _spawnerHalfCircle; // upgrade to have more creative control and use less prefabs
    [SerializeField, Space] private GameObject _spawnerOuter; // upgrade to have more creative control and use less prefabs 
    [SerializeField] private List<SpawnableEntity> _spawnableEntitiesList; // use a HashSet instead to avoid duplicates 
    [SerializeField, ConditionalShow("_invokeOnStart")] private float _invokeOnStartDelay = 10f; 
    [SerializeField, Range(5, 60)] private float _invocationDelay = 20f;
    private List<Vector3> _spawnPositionsHalfCircle = new List<Vector3>(); 
    private List<Vector3> _spawnPositionsOuter = new List<Vector3>();

    [Header("Switches")]
    [SerializeField] private List<Switch> _switchedList = new List<Switch>();
    [SerializeField, Range(1, 4)] private byte _maxActiveSwitches = 2;
    [SerializeField, Range(1, 40)] private float _vulnerabilityDuration = 20f;
    [SerializeField, Range(5, 50)] private byte _maxPercentOfDamageBeforeSwitchReset = 25;
    [SerializeField, Range(5f, 60f)] private float _switchesActivationDelay = 20f;
    [SerializeField, Range(5, 60)] private float _switchesOnDuration = 20;
    [SerializeField, 
     Tooltip("if player turns all switches on quickly, " +
             "next switches activation delay is shorter after boss goes back to attack state, " +
             "by taking into account the time it took the player to turn all the switches on ")] private bool rewardPlayerOnQuickLightsOff; 
    
    public static float sLightsOnDuration; 
    public static float sBossVulnerabilityDuration;
    public static byte sSwitchUsedCount;
    public static byte sHitCounter; 
    private SwitchesPattern _switchesPattern;
    private float _lightsActivationTimer;
    private bool switchesAreOn; 

    private StateMachine<BossStates> _fsm;

    private AIAnimation _aIAnimation;
    private byte _activeSwitches; 

    public static Action<BossStates, StateTransition> OnRequireStateChange;

    private float _invocationSelector; 
    private float _entityToInvokeSelector;
 
    private bool _canInvoke = true; 
    private bool _canDoAirAttack = false;
    private BossStates currentState, previousState;
    public static bool sAllLightsWereOff;
    public static Action OnBossVulnerable; // deactivate ray colliders; 
    private bool rayHasBeenResetAfterVulnerableState; 

    [Header("DEBUG")]
    public bool doSpawns = true;

    public LayerMask playerLayer;
    private bool _canRevealSwitches = true;  // this should not be here 
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
        previousState = currentState; 
        currentState = newState; 
        _fsm.ChangeState(newState, transition);
    }

    void Start() 
    { 
        Debug.Log("start");
        sLightsOnDuration = _switchesOnDuration;
        sSwitchUsedCount = 0;
        _bossCollider.enabled = false;
        sHitCounter = 0; 

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

        StartCoroutine(nameof(SetInvocationCooldown));
    }

    private void FixedUpdate() 
    { 
        if (!PlayerMovement_Alan.sPlayer && Time.time >= 1f) // to avoid it on first frame
        {
            OnRequireStateChange(BossStates.Default, StateTransition.Safe); 
        }

        if (switchesAreOn)
        {
            _lightsActivationTimer += 0.02f; 
        }

        if (_canInvoke)
        {
            StartCoroutine(nameof(SetInvocationCooldown)); 
            OnRequireStateChange(BossStates.Invocation, StateTransition.Safe); 
        }

        if (_bossHP.CurrentValue <= 0)
        {
            OnRequireStateChange(BossStates.Death, StateTransition.Safe); 
        }

        if (!_canRevealSwitches) return;
        SelectSwitchesPattern();
    }

    private void SelectSwitchesPattern() // remove from here.. It is not part of the state machine 
    {
        switchesAreOn = true; 
        var selector = (SwitchesPattern) Random.Range(0, (int) SwitchesPattern.FullLeft + 1);
        switch (selector) 
        {
            case SwitchesPattern.LineOne:
                _switchedList[0].ShowSwitchIsOn();
                _switchedList[1].ShowSwitchIsOn();
                break;
            case SwitchesPattern.LineTwo:
                _switchedList[2].ShowSwitchIsOn();
                _switchedList[3].ShowSwitchIsOn();
                break;
            case SwitchesPattern.LineThree:
                _switchedList[4].ShowSwitchIsOn();
                _switchedList[5].ShowSwitchIsOn();
                break;
            case SwitchesPattern.FullLeft:
                _switchedList[0].ShowSwitchIsOn();
                _switchedList[4].ShowSwitchIsOn();
                break;
            case SwitchesPattern.FullRight:
                _switchedList[1].ShowSwitchIsOn();
                _switchedList[3].ShowSwitchIsOn();
                break;
            case SwitchesPattern.DiagonalOne:
                _switchedList[0].ShowSwitchIsOn();
                _switchedList[5].ShowSwitchIsOn();
                break;
            case SwitchesPattern.DiagonalTwo:
                _switchedList[1].ShowSwitchIsOn();
                _switchedList[4].ShowSwitchIsOn();
                break;
        }
        StartCoroutine(nameof(SetSwitchesCooldown));
    }

    #endregion

#region FSM

#region Init
    void Init_Enter() 
    {
        Debug.Log("Initializing"); 
        // _aIAnimation = _graphics.GetComponent<AIAnimation>(); 
    }

    void Init_Exit()
    {
        
        
    }
    
#endregion 
    
#region Default

    void Default_Enter()
    {
        Debug.Log("default state");
    } 
    
#endregion

#region Invocation

    void Invocation_Enter()
    {
        Debug.Log("invocation enter");
        InvokeEntity(1f); 
    } 

    void Invocation_Exit()
    { 
        
    }
    
#endregion

#region Object Falling

    void ObjectFalling_Enter()
    {
        Debug.Log("object falling enter");
    }

    void ObjectFalling_FixedUpdate()
    {
        
    }

    void ObjectFalling_Exit()
    {
        
    }

#endregion 

#region Vulnerable 
    void Vulnerable_Enter()
    { 
        Debug.Log("vulnerable enter");
        OnBossVulnerable(); 
        sSwitchUsedCount = 0;
        rayHasBeenResetAfterVulnerableState = false; 
        
        _bossCollider.enabled = true; 
        sAllLightsWereOff = true; 
        BossEventProjectileFalling.sProjectileCanFall = false;
        RayAttack.sCanRayAttack = false; 
        _canRevealSwitches = false; 
        _canInvoke = false; 
        
        StartCoroutine(nameof(ResetToAttackState)); 
    }

    IEnumerator ResetToAttackState()
    {
        yield return new WaitForSeconds(_vulnerabilityDuration);
        OnRequireStateChange(BossStates.Invocation, StateTransition.Safe);
    } 
    
    void Vulnerable_FixedUpdate()
    {
        if (sHitCounter >= (_bossHP.AgentStartinHP.Value / (100 / _maxPercentOfDamageBeforeSwitchReset)))
        {
            OnRequireStateChange(BossStates.Invocation, StateTransition.Safe); 
            Debug.Log("back to invocation from max hit count"); 
        } 
    } 

    void Vulnerable_Exit()  
    {
        sHitCounter = 0;
        
        _bossCollider.enabled = false;
        StartCoroutine(nameof(SetSwitchesCooldown));
    }
    #endregion 
    
#region Death

     private void Death_Enter()
     {
         Debug.Log("death enter");

         _bossCollider.enabled = false;
         BossEventProjectileFalling.sProjectileCanFall = false;
         RayAttack.sCanRayAttack = false;
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
                basicAIBrain.OnRequireStateChange(States.Attack, StateTransition.Safe); 
            }

            _isInvoking = false; 
        }
    }

    IEnumerator SetInvocationCooldown()
    {
        _canInvoke = false;

        yield return new WaitForSeconds(_invocationDelay);
        Debug.Log($"setting can invoke to {currentState != BossStates.Vulnerable}"); 
        _canInvoke = currentState != BossStates.Vulnerable;
    } 
    
    // DRY 
    IEnumerator SetSwitchesCooldown()
    {
        _canRevealSwitches = false;
        sAllLightsWereOff = false;
        switchesAreOn = false; 
        _lightsActivationTimer = 0f;

        if (!rayHasBeenResetAfterVulnerableState)
        {
            yield return null;
            rayHasBeenResetAfterVulnerableState = true; 
            RayAttack.sCanRayAttack = true; 
        }

        yield return new WaitForSeconds(sAllLightsWereOff && rewardPlayerOnQuickLightsOff
            ? _switchesActivationDelay + _switchesOnDuration - _lightsActivationTimer
            : _switchesActivationDelay + _switchesOnDuration);
        
        Debug.Log($"setting can reveal switches to {currentState != BossStates.Vulnerable}");
        _canRevealSwitches = currentState != BossStates.Vulnerable; 
    }

    #endregion
}
