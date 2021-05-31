using System;
using System.Collections;
using System.Collections.Generic;
using BEN.Animation;
using UnityEngine;
using Random = UnityEngine.Random;

public class RayAttack : MonoBehaviour
{
    [SerializeField] private AIAnimation _bossAnimation;
    [SerializeField, Range(0f, 3f), Tooltip("delay between animation " +
                                            "and ray effectively dealing damages")] private float _rayPrewarningDuration = 1f;  
    [SerializeField, Range(0.5f, 5f)] private float _rayDamageDuration = 1.5f;  
    [SerializeField, Range(2f, 10f)] private float _delayBetwenenRayAttacks = 5f;  

    public List<GameObject> _rayPlaceholderVisuals = new List<GameObject>(); 
    private List<Collider> _rayColliders = new List<Collider>();
    private List<MeshRenderer> _rayMeshRenderers = new List<MeshRenderer>();
    public static bool sCanRayAttack;

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
        sCanRayAttack = true; 
        for (int i = 0; i < _rayPlaceholderVisuals.Count; i++)
        {
            _rayMeshRenderers.Add(_rayPlaceholderVisuals[i].GetComponent<MeshRenderer>()); 
            _rayColliders.Add(_rayPlaceholderVisuals[i].GetComponent<Collider>());  
        }
    }

    private void FixedUpdate()
    { 
        if (!sCanRayAttack) return; 
        Debug.Log(" rotating for ray attack");
        StartCoroutine(nameof(CastRayToPlayer));
        StartCoroutine(nameof(SetCanRotate));

        transform.rotation = Quaternion.Euler(0f, Random.Range(-11, 11), 0f); // UPRGADE : less random, aim at player with some accuracy value

        for (var i = 0; i < _rayPlaceholderVisuals.Count; i++)
        {
            _rayMeshRenderers[i].enabled = true;
            _rayColliders[i].enabled = false;
        } 
    } 
    
    private IEnumerator CastRayToPlayer()
    {
        
        _bossAnimation.PlayAnimation(AnimState.Atk, AnimDirection.None);
        yield return new WaitForSeconds(_rayPrewarningDuration);   
        Debug.Log("ray attacking");  

        for (var i = 0; i < _rayPlaceholderVisuals.Count; i++)
        { 
            _rayMeshRenderers[i].enabled = false;  
            _rayColliders[i].enabled = true; 
        }

        yield return new WaitForSeconds(_rayDamageDuration);  
        for (var i = 0; i < _rayPlaceholderVisuals.Count; i++)
        { 
            _rayColliders[i].enabled = false; 
        }
    } 

    private IEnumerator SetCanRotate()
    {
        sCanRayAttack = false; 
        yield return new WaitForSeconds(_delayBetwenenRayAttacks); 
        sCanRayAttack = !BossAIBrain.sAllLightsWereOff; 
    }
    
    private void DisableRayOnBossVulnerable()
    {
        for (var i = 0; i < _rayMeshRenderers.Count; i++)
        {
            _rayMeshRenderers[i].enabled = false; 
            _rayColliders[i].enabled = false; 
        }
    }
}
