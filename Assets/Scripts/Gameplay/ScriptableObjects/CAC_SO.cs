using UnityEngine;

[CreateAssetMenu(fileName = "New_CAC", menuName = "Gameplay/CAC")]
public class CAC_SO : Gameplay_SO
{
    [SerializeField, Range(1, 10)] private byte m_Damage = 1;

    private void OnEnable()
    {
        GizmosColor = Color.red;
    }
}
