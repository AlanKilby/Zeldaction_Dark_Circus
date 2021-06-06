using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using BEN.Animation;
using MonsterLove.StateMachine;

[DefaultExecutionOrder(5)]
public class BossEventProjectileFalling : MonoBehaviour
{
    [SerializeField] private GameObject[] _bossProjectile = new GameObject[2];
    [SerializeField] private GameObject _projectileShadow;
    [SerializeField, Range(0f, 2f)] private float _groundHeight = 0.25f; 
    [SerializeField] private AIAnimation _bossAnimation;
    [SerializeField] private BoxCollider _spawnZone;
    [SerializeField, Range(0f, 5f)] private float _fallZoneBorder = 1f;
    [SerializeField, Range(2f, 60f)] private float _projectileFallDelay = 30f;

    [SerializeField, Range(0, 5), Tooltip("0 means total accuracy")] private float _accuracyModifier = 2;  
    public static bool sProjectileCanFall; 
    private Vector3 projectileSpawnPosition;
    [SerializeField] private Health _bossHP;

    private GameObject projectileRef;
    public static sbyte sProjectileDirection;

    private void Start()
    {
        StartCoroutine(nameof(SetProjectileCanFall));
        sProjectileDirection = 0; 
    }  

    private void FixedUpdate()
    { 
        if (sProjectileCanFall && _bossHP.CurrentValue > 0) 
        {
            StartCoroutine(nameof(SetProjectileCanFall));
            SetProjectileSpawnPosition();
            _bossAnimation.PlayAnimation(AnimState.SecondaryAtk, sProjectileDirection == -1 ? AnimDirection.Left : AnimDirection.Right);
        }
    } 
 
    private void SetProjectileSpawnPosition()
    {
        projectileSpawnPosition = PlayerMovement_Alan.sPlayerPos + new Vector3(0f, _spawnZone.bounds.center.y, 0f) + (Random.insideUnitSphere *  _accuracyModifier);
        Mathf.Clamp(projectileSpawnPosition.x, _spawnZone.bounds.min.x, _spawnZone.bounds.min.x); 
        Mathf.Clamp(projectileSpawnPosition.z, _spawnZone.bounds.min.z, _spawnZone.bounds.min.z);

        sProjectileDirection = (sbyte)Mathf.Sign(projectileSpawnPosition.x - BossAIBrain.sBossPosition.x);
        StartCoroutine(nameof(ProjectileFall)); 
    }

    private IEnumerator ProjectileFall()
    {
        yield return new WaitForSeconds(1f);
        projectileRef = Instantiate(_bossProjectile[Random.Range(0, _bossProjectile.Length)], projectileSpawnPosition, Quaternion.identity);

        var shadowRef = Instantiate(_projectileShadow,
            new Vector3(projectileSpawnPosition.x, _groundHeight, projectileSpawnPosition.z),
            Quaternion.Euler(90f, 0f, 0f));
        shadowRef.GetComponent<Grow>().isSpotlightShadow = true; 
        
    }

    private IEnumerator SetProjectileCanFall()
    {
        sProjectileCanFall = false;
        if (BossAIBrain.sCurrentState != BossStates.Vulnerable)
        {
            Debug.Log("setting state to object falling");
            BossAIBrain.sCurrentState = BossStates.ObjectFalling;
        }

        yield return new WaitForSeconds(_projectileFallDelay);
        sProjectileCanFall = !BossAIBrain.sAllLightsWereOff &&
                             BossAIBrain.sCurrentState != BossStates.Vulnerable &&
                             BossAIBrain.sCurrentState != BossStates.RayAttacking &&
                             BossAIBrain.sCurrentState != BossStates.Invocation &&
                             _bossHP.CurrentValue > 0;

        BossAIBrain.OnRequireStateChange(BossStates.Default, StateTransition.Safe);
    }
} 
  