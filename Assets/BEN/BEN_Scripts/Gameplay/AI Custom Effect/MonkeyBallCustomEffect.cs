using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonkeyBallCustomEffect : AICustomEffectOnZoneSlow
{
    public override void InvokeCustomEvent()
    {
        Debug.Log($"custom effect on {gameObject.name}");
    }
}
