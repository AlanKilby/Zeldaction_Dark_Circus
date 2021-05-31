using System;
using BEN.AI;
using BEN.Animation;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

[Serializable]
public class BossHpLossEvent : UnityEvent<AnimState, AnimDirection> { }

[DefaultExecutionOrder(12)]  
public class Health : MonoBehaviour
{
    [SerializeField] private AgentGameplayData _agentStartingHP; 
    public sbyte CurrentValue { get; set; } // only for mobs. Player current value should be stored in a scriptable object that inherits from AgentGameplayData
    public bool IsMonkeyBall { get; set; }
    public AgentGameplayData AgentStartinHP { get => _agentStartingHP; }

    public static Action OnPlayerDeath;
    private bool _notifiedDeath; // replace with global game state to avoid same bool spread accross codebase
    private Collider _playercollider;
    private BasicAIBrain _brain;
    public Action OnMonkeyBallTransitionToNormalMonkey;
    [Space, SerializeField] private BossHpLossEvent _OnBossHPLoss;
    [SerializeField] private bool isBossHP;
    
    [Header("Debug")]
    [SerializeField] private bool _playerUnkillable; 


    private void Start()
    {
        CurrentValue = _agentStartingHP.Value;
        _brain = IsMonkeyBall ? GetComponent<BasicAIBrain>() : null; 
    }

    public void DecreaseHp(sbyte value)
    {
        CurrentValue -= value;

        if (CurrentValue > 0)
        {
            if (isBossHP) 
            {
                _OnBossHPLoss.Invoke(AnimState.Hit, Random.Range(0, 2) == 0 ? AnimDirection.Left : AnimDirection.Right);  
            }
            return;
        }

        if (_notifiedDeath) return; // avoid call if HP == -1; 
        
        if (isBossHP) 
        {
            _OnBossHPLoss.Invoke(AnimState.Die, AnimDirection.None); 
        }
        else if (!IsMonkeyBall && !_playerUnkillable)
        {
            _playercollider = GetComponent<Collider>();
            _playercollider.enabled = false; 
            OnPlayerDeath();  
        }
        else if (_brain.Type == AIType.MonkeySurBall)
        {
            OnMonkeyBallTransitionToNormalMonkey(); 
        }
        _notifiedDeath = true;
    } 
}
