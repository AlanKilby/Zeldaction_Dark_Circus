using UnityEngine;
using System.Collections.Generic; 

[System.Serializable]
public class Gameplay_SO : ScriptableObject
{
    [TextArea(2, 20), SerializeField] private string m_Description = "description..";
    [SerializeField, Range(0f, 15f)] private float m_Range = 0f;
    public float Range { get => m_Range; }

    public Color GizmosColor { get; set; }
    public bool IsLoaded { get; set; }

    public LayerMask m_InteractableLayers; 

}
