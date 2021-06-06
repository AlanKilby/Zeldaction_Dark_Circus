using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class AnimEventInstantiateObject : MonoBehaviour
{
    [SerializeField] private bool useSecondarySpawnPoint;
    [SerializeField] private bool instantiateManuallyOnSelfDestroy;
    [SerializeField, ConditionalShow("instantiateManuallyOnSelfDestroy", true)] private bool destroyNestedPrefabAfterDelay;

    [SerializeField] private bool randomSelectionFromList; 
    [SerializeField, ConditionalShow("instantiateManuallyOnSelfDestroy", true)]
    private List<GameObject> objToInstantiateOnSelfDestroy = new List<GameObject>(); 
    
    [SerializeField] private Transform originPositionReference;
    [SerializeField, ConditionalShow("useSecondarySpawnPoint", true)] private Transform secondarySpawnPoint;
    [SerializeField, ConditionalShow("instantiateManuallyOnSelfDestroy", true)] private Vector3 _eulerRotation = new Vector3(90f, 0f, 0f);
    [SerializeField, ConditionalShow("instantiateManuallyOnSelfDestroy", true)] private float _groundHeight = 0.27f;
    
    private GameObject _reference; 
    
    private void OnValidate()
    {
        if (!instantiateManuallyOnSelfDestroy)
        { 
            destroyNestedPrefabAfterDelay = false; 
        }
        
        randomSelectionFromList = objToInstantiateOnSelfDestroy.Count >= 2; 
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
        if (!Application.isPlaying) return; 
        randomSelectionFromList = objToInstantiateOnSelfDestroy.Count >= 2;
        
        if (randomSelectionFromList && instantiateManuallyOnSelfDestroy)
        {
            var selector = Random.Range(0, objToInstantiateOnSelfDestroy.Count); 
            _reference = Instantiate(objToInstantiateOnSelfDestroy[selector], 
                new Vector3(transform.position.x, _groundHeight, transform.position.z), Quaternion.Euler(_eulerRotation));
        }
        else if (instantiateManuallyOnSelfDestroy)
        {
            _reference = Instantiate(objToInstantiateOnSelfDestroy[0], 
                new Vector3(transform.position.x, _groundHeight, transform.position.z), Quaternion.Euler(_eulerRotation));
        }

        if (!destroyNestedPrefabAfterDelay) return; 
        Destroy(_reference, 1f);
    }
}
