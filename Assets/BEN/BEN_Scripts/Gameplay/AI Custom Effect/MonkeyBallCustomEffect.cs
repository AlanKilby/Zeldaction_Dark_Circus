using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonkeyBallCustomEffect : AICustomEffectOnZoneSlow
{
    public override void InvokeCustomEventOnEnter()
    {
        Debug.Log($"custom effect on {gameObject.name}");
    }
    
    public override void InvokeCustomEventOnExit()
    {
        Debug.Log($"custom effect exit on {gameObject.name}");
    }
}
