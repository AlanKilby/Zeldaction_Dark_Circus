using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_LockedDoorDetectionZone : MonoBehaviour
{
    [Tooltip("Name of this door.")]
    public string DoorName;

    [Tooltip("ID of this door.")]
    public int doorID;

    public GameObject door;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            AK_KeyManager playerKeyList = other.GetComponent<AK_KeyManager>();

            for(int i = 0; i < playerKeyList.keyIDList.Length; i++)
            {
                if (playerKeyList.keyIDList[i] == doorID)
                {
                    OpenDoor();
                    break;
                }
            }
        }
    }


    public void OpenDoor()
    {
        // WIP
        Destroy(door);
    }
}
