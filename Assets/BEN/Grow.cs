using UnityEngine;

public class Grow : MonoBehaviour
{
    [SerializeField, Range(0.2f, 3f)] private float _growthSpeed = 0.75f;
    void Start() 
    {
        _growthSpeed *= 0.02f; 
    }

    void FixedUpdate() 
    {
        transform.localScale = new Vector3(transform.localScale.x + _growthSpeed, transform.localScale.y + _growthSpeed, transform.localScale.z); 
    }
}
