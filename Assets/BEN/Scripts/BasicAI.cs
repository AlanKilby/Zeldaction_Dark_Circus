using UnityEngine;

public enum AIState { Patroling = 0, Attacking = 1 }
public class BasicAI : MonoBehaviour
{
    public LayerMask playerLayer;
    [Range(0.25f, 10f)] public float attackMoveSpeed = 3f;
    private SphereCollider selfCollider; 
    private AIState state;
    private Collider[] detectedCollider;
    private Pathfinding_Waypoints pathfinding; 

    private void Start()
    {
        pathfinding = GetComponentInParent<Pathfinding_Waypoints>();
        selfCollider = GetComponent<SphereCollider>(); 
    }

    private void FixedUpdate()
    {
        state = (AIState)BoolToInt(Physics.CheckSphere(transform.position, selfCollider.radius, playerLayer));
        detectedCollider = Physics.OverlapSphere(transform.position, selfCollider.radius, playerLayer, QueryTriggerInteraction.Collide);

        if (detectedCollider.Length != 0)
        {
            if (Vector3.Distance(transform.position, detectedCollider[0].transform.position) > 2f)
            {
                pathfinding.transform.Translate((detectedCollider[0].transform.position - transform.position).normalized * Time.fixedDeltaTime * attackMoveSpeed, Space.Self);
            }
            else
                Attack(); // use class-based state machine instead 
        } 
        else
        {
            pathfinding.FollowWaypoints(pathfinding.transform); 
        } 
    }

    private int BoolToInt(bool value) => value == true ? 1 : 0; 

    private void Attack()
    {
        Debug.Log("Attacking player"); 
    }

}
