using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_KeyManager : MonoBehaviour
{
    [Tooltip("List of the keys the player has by Name.")]
    public string[] keyNameList;

    [Tooltip("List of the keys the player has by ID.")]
    public int[] keyIDList;
}
