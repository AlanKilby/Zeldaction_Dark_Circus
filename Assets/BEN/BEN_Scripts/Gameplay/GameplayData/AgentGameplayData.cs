using UnityEngine;

[CreateAssetMenu(fileName = "New Health", menuName = "Gameplay/Health")]
public class AgentGameplayData : ScriptableObject
{
    [SerializeField, Range(1, 100)] private sbyte _startingHealth = 2; 
    public sbyte Value { get => _startingHealth; } 
} 
