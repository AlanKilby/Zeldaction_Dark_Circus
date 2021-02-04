using UnityEngine;

[ExecuteInEditMode]
public class Grapling : Gameplay 
{
    public Gameplay_SO grapling_SO;
    public Transform Target { get; private set; }

    private GameObject objToInstantiate;
    private GameObject referenceObj; 
    private Rigidbody referenceRb;

    private void OnDrawGizmos()
    {
        if (GameplayMaker.s_bGrapling)
        {
            Gizmos.color = GameplayMaker.s_GameplayActionGrapling.GetColor();
            Gizmos.DrawWireSphere(transform.position, GameplayMaker.s_GameplayActionGrapling.GetRange());
        }
    }

    public void OnEnable()
    {
        if (!grapling_SO)
            grapling_SO = Resources.Load<Grapling_SO>("New_Grapling");

        grapling_SO.IsLoaded = true;

        if (!Target)
            Target = GameObject.Find("Target").transform; 
    }

    public void OnDisable()
    {
        grapling_SO.IsLoaded = false;
    }

    private void FixedUpdate()
    {
        Target.localPosition = new Vector3(ProcessInputs.s_Direction.x, 0f, ProcessInputs.s_Direction.z) * grapling_SO.Range;
    }

    public override void DoAction()
    {
        if (!objToInstantiate)
            objToInstantiate = Resources.Load<GameObject>("Hook"); 

        referenceObj = Instantiate(objToInstantiate, Target.position, Quaternion.identity);
        referenceObj.GetComponent<DragPlayer>().SetInteractableLayers(grapling_SO.m_InteractableLayers);
        referenceObj.GetComponent<DragPlayer>().SetOrigin(transform.position); 

        Destroy(referenceObj, 2f); 
    }

    public override float GetRange() => grapling_SO.Range;

    public override Color GetColor() => grapling_SO.GizmosColor;
}
