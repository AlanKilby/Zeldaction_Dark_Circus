using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YMI_RoomHidder : MonoBehaviour
{
    [SerializeField]
    private GameObject blackPlane;

    private float curveTime;

    public AnimationCurve FadeOutCurve;
    private float timer = Time.deltaTime;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("passed 1");

        if (other.CompareTag("Player"))
        {
            Debug.Log("passed 2");
            Color transparent = this.GetComponent<MeshRenderer>().material.GetColor("_BaseColor");

            float curveTime = FadeOutCurve.Evaluate(timer);
            
            for (float alpha = 1; alpha > 0; alpha--)
            {

            }

        }
    }
            
        
            

            //Color temp = blackPlane.GetComponent<MeshRenderer>().material.GetColor("_BaseColor");
            //float fadeOut = FadeOutCurve.Evaluate(curveTime);
        
            //temp.a = fadeOut * temp.a;
            //blackPlane.GetComponent<MeshRenderer>().material.SetColor("_BaseColor",temp);
        
}

