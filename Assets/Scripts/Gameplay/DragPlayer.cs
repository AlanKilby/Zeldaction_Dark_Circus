using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragPlayer : MonoBehaviour
{
    private SphereCollider selfCollider;
    private Collider[] detectedColliders;
    LayerMask interactableLayers;
    private Transform targetToDragTransf;
    private float timer;
    private Grapling_SO grapling_SO;

    private byte enemyLayer = 9;
    private Vector3 m_Origin;
    public GameObject debug;
    private Transform hookEnd = null;
    private Vector3 dashDirection; 

    private void OnEnable()
    {
        selfCollider = GetComponent<SphereCollider>();
        targetToDragTransf = GameObject.Find("Player").transform;
        if (!grapling_SO)
            grapling_SO = Resources.Load<Grapling_SO>("New_Grapling");

        dashDirection = ProcessInputs.s_Direction.normalized;
    }

    // use raycast so that you are dragged even though you are not touching object (going beyond it)
    private void FixedUpdate()
    {
        try
        {
            if (!grapling_SO.useAsDash)
            {
                if (Physics.Linecast(m_Origin, transform.position, out RaycastHit hitInfo, interactableLayers))
                {
                    Debug.DrawLine(m_Origin, transform.position, Color.yellow);

                    if (!hookEnd)
                        hookEnd = Instantiate(debug, hitInfo.point, Quaternion.identity).transform; // drag up to this point
                }

                if (hookEnd)
                {
                    if (timer < hitInfo.distance * 0.02f) // set duration according to player hitting something instead
                        DragTarget((hookEnd.transform.position - targetToDragTransf.position).normalized);
                    // at the end of drag 
                    else if (hitInfo.collider.gameObject.layer == enemyLayer && grapling_SO.killEnemies)
                    {
                        Destroy(hitInfo.collider.gameObject);
                    }
                }
            }
            else
            {
                if (timer < grapling_SO.dashDuration)
                    DragTarget(dashDirection);
            }
        }
        catch (System.Exception) { }

    }

    private void DragTarget(Vector3 direction) 
    {
        timer += Time.fixedDeltaTime; 
        targetToDragTransf.Translate(direction * Time.fixedDeltaTime * grapling_SO.dashForce, Space.Self); 
    }

    public void SetInteractableLayers(LayerMask layers)
    {
        interactableLayers = layers;
    }

    public void SetOrigin(Vector3 origin)
    {
        m_Origin = origin; 
    }

    private void OnDestroy()
    {
        if (hookEnd)
            Destroy(hookEnd.gameObject, 0.04f); 
    }
}
