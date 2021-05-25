using UnityEngine;

public class AnimEventInstantiateObject : MonoBehaviour
{
    [SerializeField] private Transform originPositionReference;
    [SerializeField] private Transform originRotationReference;

    public void InstantiateObject(GameObject obj) 
    {
        Instantiate(obj, originPositionReference.position, Quaternion.identity); 
    } 
}
