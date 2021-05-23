using UnityEngine;

public class MonkeyCustomEffect : AICustomEffectOnZoneSlow
{
    private float value = 1f;
    public override void InvokeCustomEventOnEnter()
    {
        Debug.Log($"custom effect enter on {gameObject.name}");
        StartCoroutine(nameof(StunMonkey)); 
    }

    public override void InvokeCustomEventOnExit()
    {
        Debug.Log($"custom effect exit on {gameObject.name}");
    }

    private System.Collections.IEnumerator StunMonkey()
    {
        yield return new WaitForSeconds(0.5f);
        AIBrain.DefaultSpeed = 0f;  
    } 
}
