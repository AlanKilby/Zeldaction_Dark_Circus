using BEN.AI;
using UnityEngine;

[DefaultExecutionOrder(12)]  
public class Health : MonoBehaviour
{
    [SerializeField] private AgentGameplayData _agentStartingHP; 
    public sbyte CurrentValue { get; set; } // only for mobs. Player current value should be stored in a scriptable object that inherits from AgentGameplayData
    public bool IsAI { get; set; }

    public static System.Action OnPlayerDeath;
    private bool _notifiedDeath; // replace with global game state to avoid same bool spread accross codebase
    private BoxCollider _playercollider;
    private BasicAIBrain _brain;
    public System.Action OnMonkeyBallTransitionToNormalMonkey; 

    private void Start()
    {
        CurrentValue = _agentStartingHP.Value;
        _brain = IsAI ? GetComponent<BasicAIBrain>() : null; 
    }

    public void DecreaseHp(sbyte value)
    {
        CurrentValue -= value; 
        Debug.Log($"decreasing hp by {value}. New value is {CurrentValue}"); 

        if (CurrentValue <= 0 && !_notifiedDeath)
        {
            if (!IsAI)
            {
                _notifiedDeath = true;
                _playercollider = GetComponent<BoxCollider>();
                _playercollider.enabled = false; 
                OnPlayerDeath();  
            }
            else if (_brain.Type == AIType.MonkeySurBall)
            {
                _notifiedDeath = true;
                OnMonkeyBallTransitionToNormalMonkey(); 
            }
        }
    } 
}
