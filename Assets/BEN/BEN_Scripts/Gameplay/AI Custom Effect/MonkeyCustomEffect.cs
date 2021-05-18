using BEN.AI;
using UnityEngine;
using UnityEngine.AI;

public class MonkeyCustomEffect : AICustomEffectOnZoneSlow
{
    private NavMeshAgent _navMeshAgent;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    } 
    
    public override void InvokeCustomEvent()
    {
        Debug.Log($"custom effect on {gameObject.name}");
        StunMonkey(); 
    }

    private void StunMonkey() 
    {
        _navMeshAgent.speed = 0f; 
    } 
}
