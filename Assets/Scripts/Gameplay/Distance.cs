using UnityEngine;

[ExecuteInEditMode]
public class Distance : Gameplay
{
    public Gameplay_SO distance_SO;
    public Transform Target { get; private set; }

    private Vector3 shootDirection;

    private GameObject objToInstantiate;
    private GameObject referenceObj;
    private Rigidbody referenceRb; 

    private void OnDrawGizmos()
    {
        if (GameplayMaker.s_bDistance)
        {
            Gizmos.color = GameplayMaker.s_GameplayActionDistance.GetColor();
            Gizmos.DrawWireSphere(transform.position, GameplayMaker.s_GameplayActionDistance.GetRange());
        }
    }

    public void OnEnable()
    {
        if (!distance_SO)
            distance_SO = Resources.Load<Distance_SO>("New_Distance");

        distance_SO.IsLoaded = true;

        if (!Target)
            Target = GameObject.Find("Target").transform; 
    }

    public void OnDisable()
    {
        distance_SO.IsLoaded = false;
    }

    private void FixedUpdate()
    {
        Target.localPosition = new Vector3(ProcessInputs.s_Direction.x, 0f, ProcessInputs.s_Direction.z) * distance_SO.Range;
        shootDirection = Target.localPosition.normalized;
        Debug.DrawRay(transform.localPosition, shootDirection, Color.cyan); 
    } 

    public override void DoAction() 
    {
        Debug.Log("-- RANGED --");
        // GameplayMaker.s_RangeDetection.radius = distance_SO.Range;
        if (!objToInstantiate)
            objToInstantiate = Resources.Load<GameObject>("Arrow"); 

        referenceObj = Instantiate(objToInstantiate, transform.position, Quaternion.identity); 
        referenceRb = referenceObj.GetComponent<Rigidbody>();
        referenceRb.AddForce(shootDirection * 10f, ForceMode.Impulse);
        referenceObj.GetComponent<Arrow>().SetInteractableLayers(distance_SO.m_InteractableLayers); 

        Destroy(referenceObj, 2f); 
    }

    public override float GetRange() => distance_SO.Range;

    public override Color GetColor() => distance_SO.GizmosColor;
}
