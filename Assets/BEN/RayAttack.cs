using System.Collections;
using System.Collections.Generic;
using BEN.Animation;
using MonsterLove.StateMachine;
using UnityEngine;
using Random = UnityEngine.Random;

public class RayAttack : MonoBehaviour
{
    [SerializeField] private AIAnimation _bossAnimation;
    [SerializeField, Range(0f, 3f), Tooltip("delay between animation " +
                                            "and ray effectively dealing damages")] private float _rayPrewarningDuration = 1f;  
    [SerializeField, Range(0.5f, 5f)] private float _rayDamageDuration = 1.5f;  
    [SerializeField, Range(2f, 10f)] private float _delayBetwenenRayAttacks = 5f;
    [SerializeField, Range(5, 35)] private byte _rayMaxAngle = 10;

    public List<GameObject> _rayVisuals = new List<GameObject>(); 
    private List<Collider> _rayColliders = new List<Collider>();
    public static bool sCanRayAttack;
    [SerializeField] private Health _bossHP;
    public static List<GameObject> sRayVisuals = new List<GameObject>();
    public List<PlayAnimationFrom8Direction> _ray;
    public static System.Action OnFireDone; 
    
    [Header("-- DEBUG -- ")] 
    public bool forceAngle;
    [SerializeField, ConditionalShow("forceAngle", true)] private float _forceAngleValue = 10f; 
    
    private void OnEnable()
    {
        BossAIBrain.OnBossVulnerable += DisableRayOnBossVulnerable;
    }

    private void OnDisable()
    {
        BossAIBrain.OnBossVulnerable -= DisableRayOnBossVulnerable;
    }

    private void Start()
    {
        for (int i = 0; i < _rayVisuals.Count; i++)
        {
            _rayColliders.Add(_rayVisuals[i].GetComponent<Collider>());  
            sRayVisuals.Add(_rayVisuals[i]);
        } 
    } 

    private void FixedUpdate()
    {
        // Debug.Log("can ray attack is " + sCanRayAttack); 
        
        if (BossAIBrain.sCurrentState == BossStates.Vulnerable || !sCanRayAttack || Time.time < _delayBetwenenRayAttacks * 0.9f) return; 
        // Debug.Log(" rotating for ray attack");
        StartCoroutine(nameof(CastRayToPlayer));
        StartCoroutine(nameof(SetCanRayAttack));

        if (!forceAngle)
        {
            transform.rotation = Quaternion.Euler(0f, Random.Range(180 - _rayMaxAngle, 180 + _rayMaxAngle + 1), 0f);
        }
        else 
        {
            var selector = Random.Range(0, 2) == 0 ? 180f - _forceAngleValue : 180f + _forceAngleValue; 
            transform.rotation = Quaternion.Euler(0f, selector, 0f); 
        }

        for (var i = 0; i < _rayVisuals.Count; i++) 
        {
            // add previsualise without applying damage
            _rayVisuals[i].SetActive(false); 
        } 
    } 
    
    private IEnumerator CastRayToPlayer()
    {
        Debug.Log("ray attacking"); 
        _bossAnimation.PlayAnimation(AnimState.Atk, AnimDirection.None);
        foreach (var item in _ray)
        {
            Debug.Log("setting show ray to true");
            item.ShowRayVisuals(true);
        } 

        yield return new WaitForSeconds(0.015f); 
        foreach (var item in _ray)
        {
            Debug.Log("setting show ray to false");
            item.ShowRayVisuals(false); 
        } 

        yield return new WaitForSeconds(_rayPrewarningDuration);

        for (var i = 0; i < _rayVisuals.Count; i++)
        {
            _rayVisuals[i].SetActive(true); 
        } 

        yield return new WaitForSeconds(_rayDamageDuration);  
        for (var i = 0; i < _rayVisuals.Count; i++)
        { 
            _rayVisuals[i].SetActive(false); 
        } 

        OnFireDone(); // notify PlayAnimationFrom8Direction 
    } 

    private IEnumerator SetCanRayAttack()
    {
        sCanRayAttack = false; 
        if (BossAIBrain.sCurrentState != BossStates.Vulnerable)
        {
            Debug.Log("setting state to ray attacking");
            BossAIBrain.sCurrentState = BossStates.RayAttacking;
        } 
        
        yield return new WaitForSeconds(_delayBetwenenRayAttacks);
        sCanRayAttack = !BossAIBrain.sAllLightsWereOff && 
                        BossAIBrain.sCurrentState != BossStates.Vulnerable &&
                        BossAIBrain.sCurrentState != BossStates.ObjectFalling &&
                        BossAIBrain.sCurrentState != BossStates.Invocation && 
                        _bossHP.CurrentValue > 0;

        if (BossAIBrain.sCurrentState != BossStates.Vulnerable)
        {
            Debug.Log("calling default from ray");
            BossAIBrain.OnRequireStateChange(BossStates.Default, StateTransition.Safe);
        } 
    }
    
    private void DisableRayOnBossVulnerable()
    {
        Debug.Log("disabling rays from boss vulnerable");
        for (var i = 0; i < _rayVisuals.Count; i++)
        {
            _rayVisuals[i].SetActive(false); 
        }
    }
}
