using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using BEN.Animation;

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
    public static bool sProjectileCanFall;
    private Vector3 projectileSpawnPosition;
    [SerializeField] private Health _bossHP;

    
    [Header("DEBUG")]
    public bool _simulateTotalPrecision;

    private void Start()
    {
        StartCoroutine(nameof(SetProjectileCanFall));  
    } 

    private void FixedUpdate()
    { 
        if (sProjectileCanFall && _bossHP.CurrentValue > 0) 
        {
            StartCoroutine(nameof(SetProjectileCanFall));
            _bossAnimation.PlayAnimation(AnimState.SecondaryAtk, Random.Range(0, 2) == 0 ? AnimDirection.Right : AnimDirection.Left);

            SetProjectileSpawnPosition(); 
        }
    }

    private void SetProjectileSpawnPosition()
    {
        projectileSpawnPosition = _simulateTotalPrecision ? PlayerMovement_Alan.sPlayerPos + new Vector3(0f, 10f, 0f) : 
                                            new Vector3(Random.Range(_spawnZone.bounds.min.x + _fallZoneBorder, _spawnZone.bounds.max.x - _fallZoneBorder), 
                                            _spawnZone.bounds.center.y, 
                                            Random.Range(_spawnZone.bounds.min.z + _fallZoneBorder, _spawnZone.bounds.max.z - _fallZoneBorder));
        
        StartCoroutine(nameof(ProjectileFall)); 
    }

    private IEnumerator ProjectileFall()
    {
        yield return new WaitForSeconds(1f);
        Instantiate(_bossProjectile[Random.Range(0, _bossProjectile.Length)], projectileSpawnPosition, Quaternion.identity);
        Instantiate(_projectileShadow,
            new Vector3(projectileSpawnPosition.x, _groundHeight, projectileSpawnPosition.z),
            Quaternion.Euler(90f, 0f, 0f));
        
    }

    private IEnumerator SetProjectileCanFall()
    {
        sProjectileCanFall = false;
        yield return new WaitForSeconds(_projectileFallDelay);
        sProjectileCanFall = !BossAIBrain.sAllLightsWereOff;
        Debug.Log($"setting projectile can fall to {sProjectileCanFall}"); 
    }
} 
