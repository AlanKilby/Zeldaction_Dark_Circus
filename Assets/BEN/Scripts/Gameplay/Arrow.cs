using UnityEngine;

public class Arrow : MonoBehaviour
{
    private SphereCollider selfCollider;
    private Collider[] detectedColliders;  
    LayerMask interactableLayers;

    private void OnEnable()
    {
        selfCollider = GetComponent<SphereCollider>(); 
    }

    private void FixedUpdate() 
    {
        /* detectedColliders = Physics.OverlapSphere(transform.position, selfCollider.radius, interactableLayers); 
        if (detectedColliders.Length > 0)
        {
            for (int i = 0; i < detectedColliders.Length; i++)
            {
                Destroy(detectedColliders[i].gameObject); 
            }
            Destroy(gameObject); 
        } */
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }

    public void SetInteractableLayers(LayerMask layers)
    {
        interactableLayers = layers; 
    }
}
