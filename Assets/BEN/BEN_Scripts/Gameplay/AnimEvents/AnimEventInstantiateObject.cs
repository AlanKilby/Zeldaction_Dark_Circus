using UnityEngine;

public class AnimEventInstantiateObject : MonoBehaviour
{
    [SerializeField] private GameObject objToInstantiate; 
    [SerializeField] private Transform originPositionReference;
    [SerializeField] private Transform originRotationReference;

    public void InstantiateObject(GameObject obj) 
    {
        Instantiate(objToInstantiate, originPositionReference.position, originRotationReference.rotation);
    } 
}
