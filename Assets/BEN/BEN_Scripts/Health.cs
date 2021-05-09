using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private AgentGameplayData _agentHp;

    private void Start()
    {
        _agentHp.Initialise(); 
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            DecreaseHp(1); 
        }
    }

    public void DecreaseHp(sbyte value)
    {
        _agentHp.CurrentHealth -= value;
        Debug.Log($"decreasing hp by {value}. New value is {_agentHp.CurrentHealth}"); 
    } 
}
