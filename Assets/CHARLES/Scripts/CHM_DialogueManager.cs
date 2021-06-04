using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CHM_DialogueManager : MonoBehaviour
{
    private Queue<string> sentences;

    public Text nameText;
    public Text dialogueText;

    public Animator animator;

    public GameObject dialogueBox;

    public UnityEvent rewardOnDialogueEnd;

    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue(CHM_Dialogue dialogue)
    {
        //Debug.Log("Le dialogue manager est lancé");
        animator.SetBool("IsOpen", true);
        //Debug.Log("L'animateur est lancé"); Jusque là tout va bien.
        nameText.text = dialogue.name;

        sentences.Clear();
        
        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            Debug.Log("Il n'y a plus de phrase et la dialoguebox sera supprimée");
            EndDialogue();
            //dialogueBox.SetActive(false);
            return;

        }
        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;

            yield return new WaitForSecondsRealtime(0.02f);
        }
    }

    void EndDialogue()
    {
        rewardOnDialogueEnd.Invoke();
        animator.SetBool("IsOpen", false);
    }

}