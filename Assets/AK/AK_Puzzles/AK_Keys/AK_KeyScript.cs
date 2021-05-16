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

            playerKeyList.keyIDList[playerKeyList.keyIDList.Length] = keyID;
            playerKeyList.keyNameList[playerKeyList.keyNameList.Length] = KeyName;
        }
    }
}
