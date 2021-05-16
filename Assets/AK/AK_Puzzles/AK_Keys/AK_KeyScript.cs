using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_KeyScript : MonoBehaviour
{
    public string KeyName;
    public int keyID;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            AK_KeyManager playerKeyList = other.GetComponent<AK_KeyManager>();

            playerKeyList.keyIDList.Add(keyID);
            playerKeyList.keyNameList.Add(KeyName);
            Destroy(gameObject);
        }
    }
}
