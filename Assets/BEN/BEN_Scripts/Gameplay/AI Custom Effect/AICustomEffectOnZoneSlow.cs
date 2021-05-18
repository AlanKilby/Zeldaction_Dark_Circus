using System;
using BEN.AI;
using UnityEngine;
using UnityEngine.Events; 

[RequireComponent(typeof(BasicAIBrain))]
public abstract class AICustomEffectOnZoneSlow : MonoBehaviour
{
    protected BasicAIBrain _AIBrain; 

    private void Awake()
    {
        _AIBrain = GetComponent<BasicAIBrain>();
    }

    public abstract void InvokeCustomEvent(); 
}

