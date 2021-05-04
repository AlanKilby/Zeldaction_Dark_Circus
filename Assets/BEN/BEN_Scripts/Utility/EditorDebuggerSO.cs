using UnityEngine;

[CreateAssetMenu(fileName = "DEBUG", menuName = "DEBUG")]
public class EditorDebuggerSO : ScriptableObject
{
    public byte PatrolAmount { get; set; }
    public byte BallAmount { get; set; }
}
