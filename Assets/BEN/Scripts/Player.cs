using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject projectileToThrow;
    private bool _canFire = true; 
    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.M) && _canFire)
        { 
            ThrowProjectile(); 
        }
    }

    private void ThrowProjectile()
    {
        Instantiate(projectileToThrow, transform.position, Quaternion.identity); 
    }

    private IEnumerator Cooldown()
    {
        _canFire = false; 
        yield return new WaitForSeconds(2f);
        _canFire = true; 
    }
}
