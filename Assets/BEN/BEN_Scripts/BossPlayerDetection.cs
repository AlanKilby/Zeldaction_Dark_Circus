using UnityEngine;

public class BossPlayerDetection : MonoBehaviour
{
    [SerializeField] private LayerMask _playerLayer; 
    private void OnTriggerEnter(Collider other)
    {
        if (Mathf.Pow(2f, other.gameObject.layer) != _playerLayer) return; 
        Destroy(other.transform.root.gameObject); 
    }
}
