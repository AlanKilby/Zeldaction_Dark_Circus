using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_DuckScript : MonoBehaviour
{
    public bool wasShot;

    public GameObject symbol;
    public Sprite deadDuck;
    public int ID;

    public GameObject[] otherDucks;

    public AK_DuckPuzzleManager puzzleManager;
    public SpriteRenderer duckSpriteRenderer;

    private BoxCollider duckCollider;

    //Ajout Ulric
    Animator animator;
    public string currentState;

    public string DUCK_ALIVE = "idle";
    public string DUCK_DEAD = "hit";

    public AudioSource _audioSource; 
    public AudioClip _deadClip; 
    //

    private void Start()
    {
        animator = GetComponent<Animator>();
        duckCollider = gameObject.GetComponent<BoxCollider>();
        ChangeAnimationState(DUCK_ALIVE);
    }
    private void Update()
    {
        if (wasShot)
        {
            //duckSpriteRenderer.sprite = deadDuck;
            _audioSource.PlayOneShot(_deadClip);
            ChangeAnimationState(DUCK_DEAD);
            duckCollider.enabled = false;
        }
        else
        {
            //duckSpriteRenderer.sprite = symbol;
            ChangeAnimationState(DUCK_ALIVE);
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

    public void ChangeAnimationState(string newState)
    {
        if (currentState == newState || animator.GetCurrentAnimatorStateInfo(0).IsName("playerFakeDeath")) return;

        animator.Play(newState);
        currentState = newState;
    }
}
