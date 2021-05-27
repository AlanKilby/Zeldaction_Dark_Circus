using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UD_LeverPuzzleDoor : MonoBehaviour
{
    bool doorIsOpen;

    public Animator animLeft;
    public Animator animRight;

    void Start()
    {
        doorIsOpen = false;
        animLeft.SetBool("isOpen", doorIsOpen);
        animRight.SetBool("isOpen", doorIsOpen);
    }

    public void OpenDoors()
    {
        doorIsOpen = true;
        animLeft.SetBool("isOpen", doorIsOpen);
        animRight.SetBool("isOpen", doorIsOpen);
    }
}
