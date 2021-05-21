using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakirCustomEffect : AICustomEffectOnZoneSlow
{
    public override void InvokeCustomEvent()
    {
        Debug.Log($"custom effect on {gameObject.name}");
    }
}
