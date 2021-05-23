using BEN.AI;
using UnityEngine;

[RequireComponent(typeof(BasicAIBrain))]
public abstract class AICustomEffectOnZoneSlow : MonoBehaviour
{
    protected BasicAIBrain AIBrain; 

    private void Awake()
    {
        AIBrain = GetComponent<BasicAIBrain>();
    }

    public abstract void InvokeCustomEventOnEnter();
    public abstract void InvokeCustomEventOnExit();  
}

