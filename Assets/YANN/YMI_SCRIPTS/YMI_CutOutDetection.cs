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

    //Ajout Ulric
    public RaycastHit[] hitObjects;
    Vector2 cutoutPos;
    public float cutoutSize;
    public float cutoutSmoothness;
    public Transform playerPos;
    bool playerInSight;
    public List<RaycastHit> hitObjectsMemory = new List<RaycastHit>();
    //

    //Getting the camera to avoid error 
    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    void Update()
    {
        //Not sure
        cutoutPos = mainCamera.WorldToViewportPoint(targetObject.position);
        cutoutPos.y /= (Screen.width / Screen.height);

        //not sure�
        Vector3 offset = targetObject.position - transform.position;
        //Debug.Log(offset.magnitude);
        //Creating all the rays going from the transform of the targeted object to the wall ? And extracting the magnitude value
        // to check when where to apply the sahder 
        hitObjects = Physics.RaycastAll(transform.position, offset, offset.magnitude, Wall); //Integre les murs detect�s et les retire en temps r�el

        for (int i = 0; i < hitObjects.Length; i++)
        {
            Material[] materials = hitObjects[i].transform.GetComponent<Renderer>().materials;

            for (int n = 0; n < materials.Length; n++)
            {

                materials[n].SetVector("_CutoutPos", cutoutPos);
                materials[n].SetFloat("_CutoutSize", cutoutSize);
                materials[n].SetFloat("_CutoutSmoothness", cutoutSmoothness);
            }
            if (CheckName(hitObjects[i])) //Check si le nom de l'objet est d�j� dans la liste
            {
                hitObjectsMemory.Add(hitObjects[i]); //Liste sur laquelle j'ai un controle pour reset 
            }
        }
        Invoke("CheckWallInSight", 0.2f); //V�rifie que si un mur entre le joueur et la cam�ra
        
        if (playerInSight) //Si la cam�ra ne voit plus le joueur, plus besoin de la liste des murs concern�s, on peut la clear et reset les mat des murs
        {
            ClearList();
        }
    }

    void CheckWallInSight()
    {
        if (Physics.Linecast(transform.position, playerPos.position, Wall))
        {
            playerInSight = false; //Bool�en qui d�termine si la cam�ra voit le joueur ou non
        }
        else { playerInSight = true; }

        
    }

    void ClearList()
    {
        for (int i = 0; i < hitObjectsMemory.Count; i++)
        {
            Material[] materials = hitObjectsMemory[i].transform.GetComponent<Renderer>().materials;

            for (int n = 0; n < materials.Length; n++)
            {

                materials[n].SetVector("_CutoutPos", cutoutPos);
                materials[n].SetFloat("_CutoutSize", 0.0f);
                materials[n].SetFloat("_CutoutSmoothness", 0.0f);
            }
        }
        hitObjectsMemory.Clear();
    }

    bool CheckName(RaycastHit hitObject)
    {
        foreach(RaycastHit n in hitObjectsMemory)
        {
            if (hitObject.collider.name == n.collider.name)
            {
                return false;
            }
        }
        return true;
    }

    private void OnDrawGizmos()
    {
        if (playerPos != null)
        {
            Gizmos.DrawLine(transform.position, playerPos.position);
        }
    }
}
