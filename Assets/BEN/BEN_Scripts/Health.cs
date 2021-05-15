using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private AgentGameplayData _agentStartingHP; 
    public sbyte CurrentValue { get; set; } 

    private void Awake()
    {
        Initialise(); 
    } 

    public void Initialise()
    {
        CurrentValue = _agentStartingHP.Value;
    }

    public void DecreaseHp(sbyte value)
    {
        CurrentValue -= value; 
        Debug.Log($"decreasing hp by {value}. New value is {CurrentValue}"); 
    } 
}
