using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class PlaceholderDestination : MonoBehaviour 
{
    public NavMeshAgent agent;
    [FormerlySerializedAs("myAngle")] public float angle; // to have a smooth detection rotation
    [FormerlySerializedAs("myAngle")] [SerializeField] private float myModifiedAngle;
    [FormerlySerializedAs("myAngleIndex")] public int angleIndex;

    private void FixedUpdate() 
    {
        GetRotation();
    }

    public void GetRotation()
    {
        transform.LookAt(agent.destination);
        angle = transform.rotation.eulerAngles.y; 
        myModifiedAngle = angle; 
        
        if (myModifiedAngle > 315f || myModifiedAngle < 45f)
        {
            myModifiedAngle = 0f;
        }
        else if (myModifiedAngle > 225f && myModifiedAngle < 315f)
        {
            myModifiedAngle = 270f;
        }
        else if (myModifiedAngle > 135f && myModifiedAngle < 225f)
        {
            myModifiedAngle = 180f;
        }
        else
        {
            myModifiedAngle = 90f;
        } 

        angleIndex = (int) (myModifiedAngle / 90f);
    }
}
