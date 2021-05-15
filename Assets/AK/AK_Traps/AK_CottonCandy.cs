using UnityEngine;

public class AK_CottonCandy : MonoBehaviour
{
    public string hat = "Hat"; 
    public string player = "Player"; 
        
        
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(hat))
        {
            Boomerang otherBoomerang = other.transform.GetComponent<Boomerang>();
            otherBoomerang.speed = 0;
            otherBoomerang.comebackTimer += 1;
        } 

        if (other.gameObject.layer == LayerMask.NameToLayer(player)) 
        {
            Destroy(gameObject); 
        } 
    }
}
