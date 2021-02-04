using UnityEngine;

[ExecuteInEditMode]
public class CAC : Gameplay
{
    public Gameplay_SO cac_SO; 

    private void OnDrawGizmos()
    {
        if (GameplayMaker.s_bCac)
        {
            Gizmos.color = GameplayMaker.s_GameplayActionCAC.GetColor();
            Gizmos.DrawWireSphere(transform.position, GameplayMaker.s_GameplayActionCAC.GetRange());
        }
    }

    public void OnEnable()
    {
        if (!cac_SO)
            cac_SO = Resources.Load<CAC_SO>("New_CAC");

        cac_SO.IsLoaded = true;
    }

    public void OnDisable()
    {
        cac_SO.IsLoaded = false;
    }

    /* private void FixedUpdate()
    {
        targetDetected = Physics.OverlapSphere(transform.position, GameplayMaker.s_RangeDetection.radius, cac_SO.m_InteractableLayers); 

        if (targetDetected.Length != 0 && Input.GetButtonDown("CAC"))
        {
            for (int i = 0; i < targetDetected.Length; i++) 
            {
                Destroy(targetDetected[i].gameObject, 0.25f); 
            } 
        } 
    } */

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && Input.GetButton("CAC"))
        {
            Destroy(other.gameObject, 0.25f);
        }
    }

    public override void DoAction()
    {
        Debug.Log("-- MELEE --");
        GameplayMaker.s_RangeDetection.radius = cac_SO.Range;
    }

    public override float GetRange() => cac_SO.Range;

    public override Color GetColor() => cac_SO.GizmosColor;
}

