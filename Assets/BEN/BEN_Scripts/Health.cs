using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private AgentGameplayData _agentStartingHP; 
    public sbyte CurrentValue { get; set; } // only for mobs. Player current value should be stored in a scriptable object that inherits from AgentGameplayData
    public bool IsAI { get; set; }

    public static System.Action OnPlayerDeath;
    private bool _notified; // replace with global game state to avoid same bool spread accross codebase
    private BoxCollider _playercollider; 

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

        if (!IsAI && CurrentValue <= 0 && !_notified)
        {
            _notified = true;
            _playercollider = GetComponent<BoxCollider>();
            _playercollider.enabled = false; 
            OnPlayerDeath();  
        }
    } 
}
