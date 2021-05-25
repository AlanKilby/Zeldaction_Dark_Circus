using BEN.Math;
using UnityEngine;

[RequireComponent(typeof(ParabolicFunction))]
[DefaultExecutionOrder(15)]
public class RotateSelf : MonoBehaviour
{
    [SerializeField, Range(0f, 90f)] private float _angularSpeed = 20f;
    private ParabolicFunction _parabolicFunction;

    private void Start() 
    {
        _parabolicFunction = GetComponent<ParabolicFunction>(); 
    }

    private void FixedUpdate()
    {
        transform.Rotate(transform.forward, _angularSpeed);   
    }
}
