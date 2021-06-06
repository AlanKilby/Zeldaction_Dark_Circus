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

public enum BossStates { Init, Default, Vulnerable, Invocation, ObjectFalling, RayAttacking, Death } 
public enum SwitchesPattern { LineOne, LineTwo, LineThree, FullRight, FullLeft, DiagonalOne, DiagonalTwo } 
public class BossAIBrain : MonoBehaviour
{
    [Header("Core")]
    [SerializeField] private Collider _bossCollider;
    [SerializeField] private Health _bossHP;
    [SerializeField] private AIAnimation _bossAnimation;
    [SerializeField] private AnimEventPlaySound _OnMobInvocation; 
    [SerializeField, Tooltip("Point where the Loyal appears while vulnerable")] private Transform _BossVulnerablePoint;
    [SerializeField] private GameObject _bossGraphics; 
    [SerializeField, Range(0f, 1f)] private float _delayBeforePositioningAtVulnerablePoint = 0.5f;

    [Header("Spawn")]
    [SerializeField, Space] private GameObject _spawnerHalfCircle; 
    [SerializeField, Space] private GameObject _spawnerOuter;  
    [SerializeField] private List<SpawnableEntity> _spawnableEntitiesList;  
    [SerializeField, Range(5, 60), Tooltip("delay between each invocation. Applied from start")] private float _invocationDelay = 20f;
    [SerializeField, Tooltip("avoid cognitive overload and allows player " +
                             "to focus on attacking boss")] private bool _killAllSpawnsOnLightsOff;
    private List<Vector3> _spawnPositionsHalfCircle = new List<Vector3>(); 
    private List<Vector3> _spawnPositionsOuter = new List<Vector3>();

    [Header("Switches")] 
    [SerializeField] private List<Switch> _switchedList = new List<Switch>();
    [SerializeField, Range(1, 4)] private byte _maxActiveSwitches = 2;
    [SerializeField, Range(1, 40), Tooltip("Boss vulnerability when all swithces" +
                                           "where turned off")] private float _vulnerabilityDuration = 20f;
    [SerializeField, Range(5, 50), Tooltip("max damage applicable to Boss " +
                                           "before back to invulnerable")] private byte _maxPercentOfDamageBeforeSwitchReset = 25;
    [SerializeField, Range(5f, 60f), Tooltip("delay between each switches activation. " +
                                             "Applied from start")] private float _switchesActivationDelay = 20f;
    [SerializeField, Range(5, 60), Tooltip("time during which an active switch " +
                                           "can be interacted with")] private float _switchesOnDuration = 30f;
    [SerializeField, 
     Tooltip("if player turns all switches on quickly, " +
             "next switches activation delay is shorter after boss goes back to attack state, " +
             "by taking into account the time it took the player to turn all the switches on ")] private bool rewardPlayerOnQuickLightsOff; 
    [SerializeField, Tooltip("When false, switches will always " +
                             "get activated in the same order")] private bool _selectRandomPatttern;

    private SwitchesPattern _scriptedPatternIndex; 
    
    public static float sLightsOnDuration; 
    public static float sBossVulnerabilityDuration;
    public static byte sSwitchUsedCount;
    public static byte sHitCounter; 
    public static byte sMaxActiveSwitches;
    private SwitchesPattern _switchesPattern;
    private float _lightsActivationTimer;
    private bool switchesAreOn; 

    private StateMachine<BossStates> _fsm;

    private byte _activeSwitches; 

    public static Action<BossStates, StateTransition> OnRequireStateChange;

    private float _invocationSelector; 
    private float _entityToInvokeSelector;
 
    private bool _canInvoke = true; 
    private bool _canDoAirAttack = false;
    public static BossStates sCurrentState, previousState;
    public static bool sAllLightsWereOff;
    public static Action OnBossVulnerable; // deactivate ray colliders; 
    private bool rayHasBeenResetAfterVulnerableState;
    private bool _deathNotified; 
    private Vector3 _initialPosition;

    [SerializeField] private List<MonoBehaviour> _behavioursToDisableOnBossDeath = new List<MonoBehaviour>();

    [Header("DEBUG")]
    public bool doSpawns = true;
    public bool doSwitchMechanic = true; 
    public LayerMask playerLayer;
    private bool _canRevealSwitches;  // this should not be here 
    private bool _isInvoking;
    private List<GameObject> _invokedEntities = new List<GameObject>();
    [Tooltip("Boss wont go out from vulnerability even after getting hit " + 
             "more than (100 / _maxPercentOfDamageBeforeSwitchReset) times")] public bool ignoreMaxHitCount;

    private bool isFirstCall = true; 
    ulong seed = 61829450;
    public static Vector3 sBossPosition; 


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
        previousState = sCurrentState; 
        sCurrentState = newState; 
        _fsm.ChangeState(newState, transition);
    }

    void Start() 
    { 
        // Debug.Log("start");
        sLightsOnDuration = _switchesOnDuration;
        sSwitchUsedCount = 0;
        _bossCollider.enabled = false;
        sHitCounter = 0;
        sMaxActiveSwitches = _maxActiveSwitches;
        _bossAnimation.PlayAnimation(AnimState.Idle, AnimDirection.None);
        _initialPosition = transform.position;
        sBossPosition = transform.position; 

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
        StartCoroutine(nameof(SetSwitchesCooldown)); 
    }

    private void FixedUpdate()
    {
        Debug.Log("current state is " + sCurrentState);
        if (_deathNotified || (!PlayerMovement_Alan.sPlayer && Time.time >= 1f)) return; 
        
        if (switchesAreOn)
        {
            _lightsActivationTimer += 0.02f; 
        }

        if (_bossHP.CurrentValue <= 0 && !_deathNotified)
        {
            _deathNotified = true; 
            OnRequireStateChange(BossStates.Death, StateTransition.Safe); 
        } 
        else if (_canInvoke)
        { 
            StartCoroutine(nameof(SetInvocationCooldown)); 
            OnRequireStateChange(BossStates.Invocation, StateTransition.Safe); 
        }

        if (!_canRevealSwitches) return;
        SelectSwitchesPattern();
    }

    #endregion 

#region FSM

#region Init
    void Init_Enter() 
    {
        Debug.Log("Initializing");
        sCurrentState = BossStates.Init;
        // _aIAnimation = _graphics.GetComponent<AIAnimation>(); 
    }

    void Init_Exit()
    {
        
        
    }
    
#endregion 
    
#region Default

    void Default_Enter()
    {
        sCurrentState = BossStates.Default;
        Debug.Log("default state");
    } 
    
#endregion

#region Invocation

    void Invocation_Enter()
    {
        sCurrentState = BossStates.Invocation;
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
        sCurrentState = BossStates.ObjectFalling;
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
    IEnumerator Vulnerable_Enter()
    {
        sCurrentState = BossStates.Vulnerable;
        Debug.Log("vulnerable enter");
        try // DEBUG because will throw error when RayAttacks_Manager is disabled 
        {
            OnBossVulnerable(); 

        }
        catch (NullReferenceException) { } 

        sSwitchUsedCount = 0;
        StartCoroutine(nameof(SetSwitchesCooldown)); 
        rayHasBeenResetAfterVulnerableState = false;

        if (_killAllSpawnsOnLightsOff)
        {
            try
            {
                foreach (var item in _invokedEntities)
                {
                    item.GetComponentInChildren<Health>().DecreaseHp(100); 
                } 
            }
            catch (Exception)  { } 
        } 
        
        _bossCollider.enabled = sAllLightsWereOff = true;
        BossEventProjectileFalling.sProjectileCanFall = RayAttack.sCanRayAttack = false;
        _canRevealSwitches = _canInvoke = false; 
        
        StartCoroutine(nameof(ResetToAttackState));

        yield return new WaitForSeconds(_delayBeforePositioningAtVulnerablePoint); 
        Debug.Log("setting new position");
        _bossAnimation.PlayAnimation(AnimState.Hit, AnimDirection.None); 
        
        // had to unparent graphics from entity because of weird distortions during animations..
        transform.position = _bossGraphics.transform.position = _BossVulnerablePoint.position;  
    }

    IEnumerator ResetToAttackState()
    { 
        yield return new WaitForSeconds(_vulnerabilityDuration);
        Debug.Log("reseting to invocation state");
        StartCoroutine(nameof(SetInvocationCooldown));  
        OnRequireStateChange(BossStates.Invocation, StateTransition.Safe);
    } 
    
    void Vulnerable_FixedUpdate()
    {
        if (ignoreMaxHitCount) return; 
        if (sHitCounter >= _bossHP.AgentStartinHP.Value / (100 / _maxPercentOfDamageBeforeSwitchReset))
        {
            sHitCounter = 0; 
            _bossCollider.enabled = false;

            StartCoroutine(nameof(SetInvocationCooldown)); 
            OnRequireStateChange(BossStates.Invocation, StateTransition.Safe); 

            Debug.Log("back to invocation from max hit count"); 
        } 
    } 

    IEnumerator Vulnerable_Exit()  
    { 
        sHitCounter = 0; 
        _bossCollider.enabled = false;
        sSwitchUsedCount = 0;

        if (_bossHP.CurrentValue > 0)
        { 
            Debug.Log("playing reset anim"); 
            transform.position =  _bossGraphics.transform.position = _initialPosition; // lerp 
            var clip = _bossAnimation.PlayAnimation(AnimState.Hit, AnimDirection.Top);
            yield return new WaitForSeconds(clip.clipContainer.length * 1.15f);
        } 

        yield return new WaitForFixedUpdate(); 
        StartCoroutine(nameof(SetSwitchesCooldown)); 
    } 
    #endregion 
    
#region Death

     private void Death_Enter()
     {
         sCurrentState = BossStates.Death;
         Debug.Log("death enter"); 
         foreach (var item in _behavioursToDisableOnBossDeath)
         {
             item.enabled = false; 
         }
         
         _bossAnimation.PlayAnimation(AnimState.Die, AnimDirection.None); 
         _bossCollider.enabled = false;
         BossEventProjectileFalling.sProjectileCanFall = RayAttack.sCanRayAttack = false;
     } 
    
#endregion 

#endregion 
    
#region Functionality

    private float GaussianRandSelection() 
    { 
        double sum = 0;
        for (int i = 0; i < 3; i++) 
        { 
            var holdseed = seed;
            seed ^= seed << 13;
            seed ^= seed >> 17;
            seed ^= seed << 5; 
            var r = (long) (holdseed + seed);
            sum += (double) r * (1f / 0x7FFFFFFFFFFFFFFF);
        } 

        return Mathf.RoundToInt((float)sum); 
    }
    
    
    private void InvokeEntity(float invocationProbability)
    {
        if (!doSpawns) return;
        // StartCoroutine(SetInvocationCooldown(_invocationDelay));
        _OnMobInvocation.PlaySoundSafe(SoundType.Attack);

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
                
                _invokedEntities.Add(instanceReference); 
            } 

            _isInvoking = false;
            OnRequireStateChange(BossStates.Default, StateTransition.Safe); 
        }
    }
    
    private void SelectSwitchesPattern() // remove from here.. It is not part of the state machine 
    {
        switchesAreOn = true;

            var selector = (SwitchesPattern) Random.Range(0, (int) SwitchesPattern.DiagonalTwo + 1);
            switch (_selectRandomPatttern ? selector : _scriptedPatternIndex) 
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
                    _switchedList[2].ShowSwitchIsOn();
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
            
            _scriptedPatternIndex++;
            _scriptedPatternIndex = (SwitchesPattern)Mathf.Repeat((int)_scriptedPatternIndex, (int) SwitchesPattern.DiagonalTwo + 1); 
        
        StartCoroutine(nameof(SetSwitchesCooldown));
    }

    IEnumerator SetInvocationCooldown() 
    {
        _canInvoke = false;

        yield return new WaitForSeconds(_invocationDelay);
        _canInvoke = sCurrentState != BossStates.Vulnerable && 
                     sCurrentState != BossStates.ObjectFalling && 
                     sCurrentState != BossStates.RayAttacking && 
                     _bossHP.CurrentValue > 0;
                     
        Debug.Log($"setting can invoke to {_canInvoke}");
    }
    
    IEnumerator SetSwitchesCooldown() 
    {
        _canRevealSwitches = sAllLightsWereOff = switchesAreOn = false;
        _lightsActivationTimer = 0f;

        if (!rayHasBeenResetAfterVulnerableState)
        {
            yield return null;
            rayHasBeenResetAfterVulnerableState = true; 
            RayAttack.sCanRayAttack = BossEventProjectileFalling.sProjectileCanFall = true; 
        } 

        if (isFirstCall)
        {
            isFirstCall = false; 
            yield return new WaitForSeconds(_switchesActivationDelay); 
        }
        else
        {
            yield return new WaitForSeconds(sAllLightsWereOff && rewardPlayerOnQuickLightsOff
                ? _switchesActivationDelay + _switchesOnDuration - _lightsActivationTimer
                : _switchesActivationDelay + _switchesOnDuration);
        }

        _canRevealSwitches = sCurrentState != BossStates.Vulnerable && _bossHP.CurrentValue > 0; 
        Debug.Log($"setting can reveal switches to {_canRevealSwitches}");
    }

    #endregion
}
