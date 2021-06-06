using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UD_MenuRoulottesScript : MonoBehaviour
{
    public GameObject buttonY;
    public Animator buttonAnim;

    [Header("Anims Name")]
    public string OpenAnimName;
    public string CloseAnimName;

    [Header("Input Action")]
    public UnityEvent action;

    void Start()
    {
        buttonY.SetActive(false);
        buttonAnim.Play(CloseAnimName);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            buttonY.SetActive(true);
            buttonAnim.Play(OpenAnimName);
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetButtonDown("PlayerInteraction"))
        {
            action.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            buttonY.SetActive(false);
            buttonAnim.Play(CloseAnimName);
        }
    }
}
