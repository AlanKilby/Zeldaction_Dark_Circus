using UnityEngine;

public class AK_CottonCandy : MonoBehaviour
{
    public LayerMask hatLayer;
    public LayerMask playerLayer;
        
        
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerWeapon"))
        {
            Boomerang otherBoomerang = other.transform.GetComponent<Boomerang>();
            otherBoomerang.isStunned = true;
        } 

        if (other.gameObject.layer == playerLayer) 
        {
            Destroy(gameObject); 
        } 
    }
}
