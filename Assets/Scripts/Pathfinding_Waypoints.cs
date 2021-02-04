using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding_Waypoints : MonoBehaviour
{
    public GameObject parentWaypoint;
    [Range(0.25f, 10f)] public float speed = 3f;
    private Vector3 nodeToVist;
    public List<GameObject> nodesList = new List<GameObject>();
    private byte index;
    private bool goingBack; 

    private void Start()
    {
        /* for (int i = 0; i < parentWaypoint.transform.childCount ; i++)
            nodesList.Add(parentWaypoint.transform.GetChild(i).transform.position); */
    }

    private void OnDrawGizmosSelected() 
    {
        try
        {
            for (int i = 0; i < nodesList.Count - 1; i++)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(nodesList[i].transform.position, nodesList[i + 1].transform.position); 
            }
        }
        catch (System.Exception) { }
    }

    public void FollowWaypoints(Transform targetToMove)
    {
        // ugly past-midnight solution 
        if (Vector3.Distance(targetToMove.transform.position, nodesList[index].transform.position) < 0.1f)
        {
            if (index == nodesList.Count - 1)
                goingBack = true;
            else if (index == 0)
                goingBack = false;
        }

        // no looping at the end of list
        index = Vector3.Distance(targetToMove.transform.position, nodesList[index].transform.position) < 0.1f ? (index < nodesList.Count - 1 && !goingBack ? index += 1 : index -= 1) : index; 

        targetToMove.transform.Translate((nodesList[index].transform.position - targetToMove.transform.position).normalized * Time.fixedDeltaTime * speed); 
    } 

}
