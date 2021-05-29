using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BossEventProjectileFalling : MonoBehaviour
{
    [SerializeField] private GameObject[] _bossProjectile = new GameObject[2];
    [SerializeField] private GameObject _projectileShadow;
    [SerializeField, Range(0f, 2f)] private float _groundHeight = 0.25f; 
    [SerializeField] private UnityEvent _OnSpotlightFalling;
    [SerializeField] private BoxCollider _spawnZone;
    [SerializeField, Range(0f, 5f)] private float _fallZoneBorder = 1f;
    [SerializeField, Range(2f, 60f)] private float _projectileFallDelay = 30f; 
    private bool projectileCanFall = true;
    private Vector3 projectileSpawnPosition;

    private void FixedUpdate() 
    {
        if (projectileCanFall)
        {
            StartCoroutine(nameof(SetProjectileCanFall)); 
            
             projectileSpawnPosition = new Vector3(Random.Range(_spawnZone.bounds.min.x + _fallZoneBorder, _spawnZone.bounds.max.x - _fallZoneBorder), 
                                                    _spawnZone.bounds.center.y, 
                                                    Random.Range(_spawnZone.bounds.min.z + _fallZoneBorder, _spawnZone.bounds.max.z - _fallZoneBorder));
             Instantiate(_bossProjectile[Random.Range(0, _bossProjectile.Length)], projectileSpawnPosition, Quaternion.identity);
             Instantiate(_projectileShadow,
                 new Vector3(projectileSpawnPosition.x, _groundHeight, projectileSpawnPosition.z),
                 Quaternion.Euler(90f, 0f, 0f));
             _OnSpotlightFalling.Invoke();
        }
    }

    private IEnumerator SetProjectileCanFall()
    {
        projectileCanFall = false;
        yield return new WaitForSeconds(_projectileFallDelay); 
        projectileCanFall = true; 
    }
}
