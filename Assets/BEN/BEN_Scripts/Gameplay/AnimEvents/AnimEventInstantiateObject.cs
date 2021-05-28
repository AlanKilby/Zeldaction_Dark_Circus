using UnityEngine;

public class AnimEventInstantiateObject : MonoBehaviour
{
    [SerializeField] private bool showOffset; 
    [SerializeField] private Transform originPositionReference;
    [SerializeField, ConditionalShow("showOffset", true)] private Vector3 _offsetToAdd;

    private void OnDrawGizmosSelected()
    { 
        if (!showOffset || !originPositionReference) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(originPositionReference.position, .2f); 
        Gizmos.DrawLine(originPositionReference.position, originPositionReference.position + _offsetToAdd);
    }

    public void InstantiateObject(GameObject obj) 
    {
        Instantiate(obj, originPositionReference.position, Quaternion.identity); 
    } 
    
    public void InstantiateObjectWithOffset(GameObject obj) 
    {
        Instantiate(obj, transform.TransformPoint(originPositionReference.localPosition + _offsetToAdd), Quaternion.identity); 
    } 
}
