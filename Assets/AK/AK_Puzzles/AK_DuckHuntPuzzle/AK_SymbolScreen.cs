using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_SymbolScreen : MonoBehaviour
{

    //public SpriteRenderer screen;

    public GameObject[] ducks;

    private void Start()
    {
        //screen.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!ducks[0].GetComponent<AK_DuckScript>().wasShot && !ducks[1].GetComponent<AK_DuckScript>().wasShot && !ducks[2].GetComponent<AK_DuckScript>().wasShot && !ducks[3].GetComponent<AK_DuckScript>().wasShot)
        {
            for(int i = 0; i < ducks.Length; i++)
            {
                if (i == 0)
                {
                    ducks[i].GetComponent<AK_DuckScript>().symbol.SetActive(true);
                }
                else
                {
                    ducks[i].GetComponent<AK_DuckScript>().symbol.SetActive(false);
                }
            }
        }
        else if (ducks[0].GetComponent<AK_DuckScript>().wasShot && !ducks[1].GetComponent<AK_DuckScript>().wasShot && !ducks[2].GetComponent<AK_DuckScript>().wasShot && !ducks[3].GetComponent<AK_DuckScript>().wasShot)
        {
            for (int i = 0; i < ducks.Length; i++)
            {
                if (i == 1)
                {
                    ducks[i].GetComponent<AK_DuckScript>().symbol.SetActive(true);
                }
                else
                {
                    ducks[i].GetComponent<AK_DuckScript>().symbol.SetActive(false);
                }
            }
        }
        else if (ducks[0].GetComponent<AK_DuckScript>().wasShot && ducks[1].GetComponent<AK_DuckScript>().wasShot && !ducks[2].GetComponent<AK_DuckScript>().wasShot && !ducks[3].GetComponent<AK_DuckScript>().wasShot)
        {
            for (int i = 0; i < ducks.Length; i++)
            {
                if (i == 2)
                {
                    ducks[i].GetComponent<AK_DuckScript>().symbol.SetActive(true);
                }
                else
                {
                    ducks[i].GetComponent<AK_DuckScript>().symbol.SetActive(false);
                }
            }
        }
        else if (ducks[0].GetComponent<AK_DuckScript>().wasShot && ducks[1].GetComponent<AK_DuckScript>().wasShot && ducks[2].GetComponent<AK_DuckScript>().wasShot && !ducks[3].GetComponent<AK_DuckScript>().wasShot)
        {
            for (int i = 0; i < ducks.Length; i++)
            {
                if (i == 3)
                {
                    ducks[i].GetComponent<AK_DuckScript>().symbol.SetActive(true);
                }
                else
                {
                    ducks[i].GetComponent<AK_DuckScript>().symbol.SetActive(false);
                }
            }
        }
    }
}
