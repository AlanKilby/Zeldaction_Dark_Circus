using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RayAttack : MonoBehaviour
{
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
        Debug.Log(" attacking");
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
        yield return new WaitForSeconds(2f);  
        Debug.Log("debugging ray");  

        for (var i = 0; i < _rayPlaceholderVisuals.Count; i++)
        { 
            _rayMeshRenderers[i].enabled = false; 
            _rayColliders[i].enabled = true; 
        }
    }

    private void DisableRayOnBossVulnerable()
    {
        for (var i = 0; i < _rayMeshRenderers.Count; i++)
        {
            _rayMeshRenderers[i].enabled = false; 
            _rayColliders[i].enabled = false; 
        }
    } 

    private IEnumerator SetCanRotate()
    {
        sCanRayAttack = false; 
        yield return new WaitForSeconds(5f);
        sCanRayAttack = !BossAIBrain.sAllLightsWereOff; 
    }
}
