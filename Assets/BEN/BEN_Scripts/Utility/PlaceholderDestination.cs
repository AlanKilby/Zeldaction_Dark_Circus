using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class PlaceholderDestination : MonoBehaviour 
{
    public NavMeshAgent agent;
    [FormerlySerializedAs("myAngle")] public float EulerAnglesY; // to have a smooth detection rotation
    [FormerlySerializedAs("myAngle")] [SerializeField] private float myModifiedEulerAnglesY;
    [FormerlySerializedAs("myAngleIndex")] public int angleIndex;

    private void FixedUpdate() 
    {
        GetRotation();
    }

    private void GetRotation()
    {
        transform.LookAt(agent.destination);
        EulerAnglesY = transform.rotation.eulerAngles.y; 
        myModifiedEulerAnglesY = EulerAnglesY; 
        
        if (myModifiedEulerAnglesY > 315f || myModifiedEulerAnglesY < 45f)
        {
            myModifiedEulerAnglesY = 0f;
        }
        else if (myModifiedEulerAnglesY > 225f && myModifiedEulerAnglesY < 315f)
        {
            myModifiedEulerAnglesY = 270f;
        }
        else if (myModifiedEulerAnglesY > 135f && myModifiedEulerAnglesY < 225f)
        {
            myModifiedEulerAnglesY = 180f;
        }
        else
        {
            myModifiedEulerAnglesY = 90f;
        } 

        angleIndex = (int) (myModifiedEulerAnglesY / 90f);
    }
}
