using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YMI_CutOutDetection : MonoBehaviour
{
    //getting the tranform of targeted GameObject
    [SerializeField]
    private Transform targetObject;

    //Masking walls form layer
    [SerializeField]
    private LayerMask Wall;

    private Camera mainCamera;

    //Getting the camera to avoid error 
    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    void Update()
    {
        //Not sure
        Vector2 cutoutPos = mainCamera.WorldToViewportPoint(targetObject.position);
        cutoutPos.y /= (Screen.width / Screen.height);

        //not sure²
        Vector3 offset = targetObject.position - transform.position;
        //Debug.Log(offset.magnitude);
        //Creating all the rays going from the transform of the targeted object to the wall ? And extracting the magnitude value
        // to check when where to apply the sahder 
        RaycastHit[] hitObjects = Physics.RaycastAll(transform.position, offset, offset.magnitude, Wall);

        for (int i = 0; i < hitObjects.Length; i++)
        {
            Material[] materials = hitObjects[i].transform.GetComponent<Renderer>().materials;

            for (int n = 0; n < materials.Length; n++)
            {
                
                materials[n].SetVector("_CutoutPos", cutoutPos);
                materials[n].SetFloat("_CutoutSize", 0.5f);
                materials[n].SetFloat("_CutoutSmoothness", 0.3f);
            }
        }
    }
}
