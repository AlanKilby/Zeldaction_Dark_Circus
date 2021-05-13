using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_DuckPuzzleManager : MonoBehaviour
{
    public GameObject[] ducks;
    public void DuckBackUp()
    {
        for(int i = 0; i < ducks.Length; i++)
        {
            ducks[i].GetComponent<AK_DuckScript>().wasShot = false;
        }
    }

}
