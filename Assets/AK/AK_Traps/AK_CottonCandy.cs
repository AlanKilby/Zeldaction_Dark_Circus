using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_CottonCandy : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Hat"))
        {
            Boomerang otherBoomerang = other.transform.GetComponent<Boomerang>();
            otherBoomerang.speed = 0;
            otherBoomerang.comebackTimer += 1;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Destroy(gameObject);
        }
    }
}
