using UnityEngine;

[CreateAssetMenu(fileName = "New_Distance", menuName = "Gameplay/Distance")]
public class Distance_SO : Gameplay_SO
{
    private void OnEnable()
    {
        GizmosColor = Color.green;
    }
}
