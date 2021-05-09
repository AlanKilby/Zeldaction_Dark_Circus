using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private AgentGameplayData _agentHp;

    private void Awake()
    {
        _agentHp.Initialise(); 
    } 

    public void DecreaseHp(sbyte value)
    {
        _agentHp.CurrentHealth -= value; 
        Debug.Log($"decreasing hp by {value}. New value is {_agentHp.CurrentHealth}"); 
    } 
}
