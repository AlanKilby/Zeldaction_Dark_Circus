using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_DuckScript : MonoBehaviour
{
    public bool wasShot;

    public Sprite symbol;
    public Sprite deadDuck;
    public int ID;

    public GameObject[] otherDucks;

    public AK_DuckPuzzleManager puzzleManager;
    public SpriteRenderer duckSpriteRenderer;

    private BoxCollider duckCollider;

    private void Start()
    {
        duckCollider = gameObject.GetComponent<BoxCollider>();
    }
    private void Update()
    {
        if (wasShot)
        {
            duckSpriteRenderer.sprite = deadDuck;
            duckCollider.enabled = false;
        }
        else
        {
            duckSpriteRenderer.sprite = symbol;
            duckCollider.enabled = true;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("PlayerWeapon"))
        {
            if(ID == 1 && !otherDucks[0].GetComponent<AK_DuckScript>().wasShot && !otherDucks[1].GetComponent<AK_DuckScript>().wasShot && !otherDucks[2].GetComponent<AK_DuckScript>().wasShot)
            {
                wasShot = true;
                
            }
            else if (ID == 2 && otherDucks[0].GetComponent<AK_DuckScript>().wasShot && !otherDucks[1].GetComponent<AK_DuckScript>().wasShot && !otherDucks[2].GetComponent<AK_DuckScript>().wasShot)
            {
                wasShot = true;
            }
            else if (ID == 3 && otherDucks[0].GetComponent<AK_DuckScript>().wasShot && otherDucks[1].GetComponent<AK_DuckScript>().wasShot && !otherDucks[2].GetComponent<AK_DuckScript>().wasShot)
            {
                wasShot = true;
            }
            else if (ID == 4 && otherDucks[0].GetComponent<AK_DuckScript>().wasShot && otherDucks[1].GetComponent<AK_DuckScript>().wasShot && otherDucks[2].GetComponent<AK_DuckScript>().wasShot)
            {
                wasShot = true;
            }
            else
            {
                puzzleManager.DuckBackUp();
            }
        }
    }
}
