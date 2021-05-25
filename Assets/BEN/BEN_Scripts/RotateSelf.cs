using BEN.Math;
using UnityEngine;

[DefaultExecutionOrder(15)]
public class RotateSelf : MonoBehaviour
{
    [SerializeField, Range(0f, 90f)] private float _angularSpeed = 20f;
    private ParabolicFunction _parabolicFunction;
    [SerializeField] private bool isProjectile = true; 

    private void Start()
    {
        if (!isProjectile) return; 
        _parabolicFunction = GetComponent<ParabolicFunction>(); 
    } 

    private void FixedUpdate()
    {
        // transform.Rotate(transform.forward, _angularSpeed); 
        transform.Rotate(new Vector3(0f, 0f, _angularSpeed)); 
    } 
}
