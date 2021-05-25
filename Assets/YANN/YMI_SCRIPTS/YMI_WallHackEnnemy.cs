using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YMI_WallHackEnnemy : MonoBehaviour
{
    [SerializeField]
    private LayerMask wallMask;

    [SerializeField]
    private Transform parent;

    //Spawn point of the prefab 
    [SerializeField]
    private GameObject frontShadowPOS;

    [SerializeField]
    private GameObject frontShadowOBJ;

    private bool WallContact = false;
    //private float zposition;
    //private Collider colliderBounds;
    private Vector3 instancePos = new Vector3();
    private GameObject instance;

    private void OnTriggerEnter(Collider other )
    {
        WallContact = true;
        
        if (WallContact == true)
        {
            if (((1 << other.gameObject.layer) & wallMask) != 0)
            {
                Debug.Log("c'est ça oui");

                instancePos = new Vector3(parent.position.x, parent.position.y, -10);
                
                if (instance == null)
                {
                    instance = Instantiate(frontShadowOBJ, instancePos, Quaternion.identity, parent.transform);
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        WallContact = false;
 
        if (WallContact == false)
        {
            if (((1 << other.gameObject.layer) & wallMask) != 0)
            {
                Destroy(instance.gameObject);
                instance = null;
                Debug.Log("pas ça zinedine");
            }  
        }
    }
}
