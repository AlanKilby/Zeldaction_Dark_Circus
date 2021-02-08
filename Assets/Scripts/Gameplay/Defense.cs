using UnityEngine;

[ExecuteInEditMode]
public class Defense : Gameplay
{
    public Gameplay_SO defense_SO; 

    private void OnDrawGizmos()
    {
        if (GameplayMaker.s_bDefense)
        {
            Gizmos.color = GameplayMaker.s_GameplayActionDefense.GetColor();
            Gizmos.DrawWireSphere(transform.position, GameplayMaker.s_GameplayActionDefense.GetRange());
        }
    }

    public void OnEnable()
    {
        if (!defense_SO)
            defense_SO = Resources.Load<Defense_SO>("New_Defense");

        defense_SO.IsLoaded = true;
    }

    public void OnDisable()
    {
        defense_SO.IsLoaded = false;
    }

    public override void DoAction()
    {
        Debug.Log("-- DEFENDING --");
        GameplayMaker.s_RangeDetection.radius = defense_SO.Range; 
    } 

    public override float GetRange() => defense_SO.Range;

    public override Color GetColor() => defense_SO.GizmosColor;
}
