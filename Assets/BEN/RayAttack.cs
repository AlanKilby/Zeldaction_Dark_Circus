using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RayAttack : MonoBehaviour
{
    public List<GameObject> _rayPlaceholderVisuals = new List<GameObject>(); 
    private List<Collider> _rayColliders = new List<Collider>();
    private List<MeshRenderer> _rayMeshRenderers = new List<MeshRenderer>();
    private bool canRayAttack = true; 

    private void Start()
    {
        for (int i = 0; i < _rayPlaceholderVisuals.Count; i++)
        {
            _rayMeshRenderers.Add(_rayPlaceholderVisuals[i].GetComponent<MeshRenderer>()); 
            _rayColliders.Add(_rayPlaceholderVisuals[i].GetComponent<Collider>());  
        }
    }

    private void FixedUpdate()
    {
        if (!canRayAttack) return; 
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

    private IEnumerator SetCanRotate()
    {
        canRayAttack = false; 
        yield return new WaitForSeconds(5f);
        canRayAttack = true; 
    }
}
