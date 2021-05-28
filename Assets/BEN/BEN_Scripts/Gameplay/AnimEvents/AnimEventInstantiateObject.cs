using System;
using UnityEngine;

public class AnimEventInstantiateObject : MonoBehaviour
{
    [SerializeField] private bool useSecondarySpawnPoint;
    [SerializeField] private bool instantiateManuallyOnSelfDestroy;
    [SerializeField, ConditionalShow("instantiateManuallyOnSelfDestroy", true)] private bool destroyNestedPrefabAfterDelay; 
    [SerializeField, ConditionalShow("instantiateManuallyOnSelfDestroy", true)] private GameObject objToInstantiateOnSelfDestroy; 
    [SerializeField] private Transform originPositionReference;
    [SerializeField, ConditionalShow("useSecondarySpawnPoint", true)] private Transform secondarySpawnPoint;

    private void OnValidate()
    {
        if (!instantiateManuallyOnSelfDestroy)
        { 
            destroyNestedPrefabAfterDelay = false; 
        }
    } 

    public void InstantiateObject(GameObject obj) 
    {
        Instantiate(obj, originPositionReference.position, Quaternion.identity);
    } 
    
    public void InstantiateObjectAtSecondaryPosition(GameObject obj) 
    {
        Instantiate(obj, secondarySpawnPoint.position, Quaternion.identity); 
    }

    private void OnDestroy() 
    {
        if (!Application.isPlaying || !instantiateManuallyOnSelfDestroy) return;

        var reference = Instantiate(objToInstantiateOnSelfDestroy, new Vector3(transform.position.x, 0.15f, transform.position.z), Quaternion.Euler(90f, 0f, 0f));
        if (!destroyNestedPrefabAfterDelay) return; 
        Destroy(reference, 1f);
    }
}
