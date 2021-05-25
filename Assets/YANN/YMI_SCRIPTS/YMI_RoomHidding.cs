using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YMI_RoomHidding : MonoBehaviour
{
    [SerializeField]
    private GameObject blackPlane;

    private Color Transparency;
    public float fadeOutSpeed = 0.5f;
    public int roundNumber = 25;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("tes ddans pute ?");
            Transparency = blackPlane.GetComponent<MeshRenderer>().material.GetColor("_BaseColor");
            StartCoroutine(FadeOutTime(fadeOutSpeed, roundNumber));
            blackPlane.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Transparency);
        }
    }

    private IEnumerator FadeOutTime(float speed, int rounds)
    {
        float temp = 1/(256 / rounds);
        while (Transparency.a > 0)
        {
            Debug.Log(Transparency.a);
            //Debug.Log(rounds);
            if (temp > Transparency.a) 
            {
                Transparency.a = 0;
            }
            else
            {
                Transparency.a -= temp;
            }
             Debug.Log(temp);

            yield return new WaitForSeconds(speed / rounds);
                
        }
    }
}
