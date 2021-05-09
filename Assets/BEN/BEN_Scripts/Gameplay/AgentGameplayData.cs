using UnityEngine;

[CreateAssetMenu(fileName = "New Health", menuName = "Gameplay/Health")]
public class AgentGameplayData : ScriptableObject
{
    [SerializeField, Range(1, 100)] private sbyte _startingHealth = 2; 
    public sbyte CurrentHealth { get => _startingHealth ; set => _startingHealth = value ; } 

    public void Initialise() 
    {
        CurrentHealth = _startingHealth;
        Debug.Log($"current health is : {CurrentHealth}"); 
    } 
}
