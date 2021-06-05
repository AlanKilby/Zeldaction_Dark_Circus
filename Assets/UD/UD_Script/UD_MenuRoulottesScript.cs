using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UD_MenuRoulottesScript : MonoBehaviour
{
    public GameObject buttonY;
    public Animator buttonAnim;

    [Header("Anims Name")]
    public string OpenAnimName;
    public string CloseAnimName;

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

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            buttonY.SetActive(false);
            buttonAnim.Play(CloseAnimName);
        }
    }
}
