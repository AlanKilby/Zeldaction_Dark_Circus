using UnityEngine;

[CreateAssetMenu(fileName = "New_Grapling", menuName = "Gameplay/Grapling")]
public class Grapling_SO : Gameplay_SO
{
    [Header("Hook")]
    public bool killEnemies;
    [Range(8f, 25)] public float DragForce = 20f;

    [Header("Dash")]
    public bool useAsDash;
    [Range(20f, 50f)] public float dashForce = 40f;
    [Range(0.05f, 1f)] public float dashDuration = 0.1f;

    private void OnEnable()
    {
        GizmosColor = Color.yellow;
    }
}
