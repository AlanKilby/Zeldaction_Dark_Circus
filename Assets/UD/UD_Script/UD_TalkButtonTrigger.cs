using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UD_TalkButtonTrigger : MonoBehaviour
{
    public UnityEvent DialogueTrigger;

    void Update()
    {
        if (Input.GetButtonDown("PlayerAttack"))
        {
            DialogueTrigger.Invoke();
            gameObject.SetActive(false);
        }
    }
}
