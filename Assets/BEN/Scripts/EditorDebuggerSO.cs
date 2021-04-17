using UnityEngine;
using System.Collections.Generic; 

[CreateAssetMenu(fileName = "DEBUG", menuName = "DEBUG")]
public class EditorDebuggerSO : ScriptableObject
{
    public byte Amount { get; set; }
    private List<GameObject> _gameobjList = new List<GameObject>(); 
}
