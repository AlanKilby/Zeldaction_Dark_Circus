using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParabolicFunction : MonoBehaviour
{
    [Range(1f, 10f)] public float speed = 5f;
    [Range(1f, 10f)] public float amplitude = 2f;

    private Vector3 m_Target;
    private bool targetIsSet;
    private Vector3 direction; 

    private void FixedUpdate()
    { 
        if (targetIsSet)
        {
            direction = (m_Target - transform.position).normalized; 
            transform.Translate(direction * Time.fixedDeltaTime * speed, Space.World); 
        }
    } 

    public void SetTargetPosition(Vector3 target)
    {
        targetIsSet = true; 
        m_Target = target; 
    }
}
