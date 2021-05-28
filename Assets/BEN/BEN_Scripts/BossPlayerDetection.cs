using UnityEngine;

public class BossPlayerDetection : MonoBehaviour
{
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField, Range(1, 5)] private sbyte _rayDamageAmount = 1; 
    
    private void OnTriggerEnter(Collider other)
    {
        if (Mathf.Pow(2f, other.gameObject.layer) != _playerLayer) return; 
        other.GetComponent<Health>().DecreaseHp(_rayDamageAmount);  
    }
}
