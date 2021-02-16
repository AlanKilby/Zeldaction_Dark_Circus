using UnityEngine;

[CreateAssetMenu(fileName = "New_Defense", menuName = "Gameplay/Defense")]
public class Defense_SO : Gameplay_SO
{
    [SerializeField, Range(1, 5)] private byte m_Defense = 1;
    public byte Defense { get => m_Defense; }

    private void OnEnable()
    {
        GizmosColor = Color.blue;
    }
}
