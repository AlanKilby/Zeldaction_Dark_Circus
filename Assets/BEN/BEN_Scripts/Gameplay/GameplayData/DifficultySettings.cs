using UnityEngine;

[CreateAssetMenu(fileName = "New Difficulty Settings", menuName = "Gameplay/Difficulty Settings")]
public class DifficultySettings : ScriptableObject
{
    public Difficulty Value { get; set; } 
}
