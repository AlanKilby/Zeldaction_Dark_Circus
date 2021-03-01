using UnityEngine;
using UnityEngine.AI;

// make this called by AI when player is in range, and override speed with custom one (preset stored in a ScriptableObject)
public class MoveTo : MonoBehaviour
{
    public Transform goal;
    public bool useMouse = false; 
    private NavMeshAgent agent;
    private float delayModifier = 0f;
    public static System.Action OnBallMoving; 

    private void OnEnable()
    {
        AvoidObject.OnAvoiding += MoveToTargetPosition;
        agent = GetComponent<NavMeshAgent>();
        if (!useMouse)
        {
            Invoke(nameof(SetNewGoal), 0.5f + delayModifier);
        }
    } 

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && useMouse)
        {
            TryGetMouseClickPosition(); 
        }
    }

    private void OnDisable()
    {
        AvoidObject.OnAvoiding -= MoveToTargetPosition;
    }

    void SetNewGoal() 
    {
        agent.destination = goal.position;
    }

    void TryGetMouseClickPosition()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100))
        {
            agent.destination = hit.point;
        }
    }

    private void MoveToTargetPosition(Vector3 targetPosition)
    {
        float initialHeight = transform.position.y; // to keep same height and not monkey's height
        transform.position = targetPosition - new Vector3(0f, initialHeight, 0f);

        delayModifier = 0.5f;
        OnBallMoving(); 
    }
}

