using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_SymbolScreen : MonoBehaviour
{

    public SpriteRenderer screen;

    public GameObject[] ducks;

    private void Start()
    {
        //screen.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!ducks[0].GetComponent<AK_DuckScript>().wasShot && !ducks[1].GetComponent<AK_DuckScript>().wasShot && !ducks[2].GetComponent<AK_DuckScript>().wasShot && !ducks[3].GetComponent<AK_DuckScript>().wasShot)
        {
            screen.sprite = ducks[0].GetComponent<AK_DuckScript>().symbol;
        }
        else if (ducks[0].GetComponent<AK_DuckScript>().wasShot && !ducks[1].GetComponent<AK_DuckScript>().wasShot && !ducks[2].GetComponent<AK_DuckScript>().wasShot && !ducks[3].GetComponent<AK_DuckScript>().wasShot)
        {
            screen.sprite = ducks[1].GetComponent<AK_DuckScript>().symbol;
        }
        else if (ducks[0].GetComponent<AK_DuckScript>().wasShot && ducks[1].GetComponent<AK_DuckScript>().wasShot && !ducks[2].GetComponent<AK_DuckScript>().wasShot && !ducks[3].GetComponent<AK_DuckScript>().wasShot)
        {
            screen.sprite = ducks[2].GetComponent<AK_DuckScript>().symbol;
        }
        else if (ducks[0].GetComponent<AK_DuckScript>().wasShot && ducks[1].GetComponent<AK_DuckScript>().wasShot && ducks[2].GetComponent<AK_DuckScript>().wasShot && !ducks[3].GetComponent<AK_DuckScript>().wasShot)
        {
            screen.sprite = ducks[3].GetComponent<AK_DuckScript>().symbol;
        }
    }
}
