using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_KeyManager : MonoBehaviour
{
    [Tooltip("List of the keys the player has by Name.")]
    public List<string> keyNameList = new List<string>();

    [Tooltip("List of the keys the player has by ID.")]
    public List<int> keyIDList = new List<int>();
}
