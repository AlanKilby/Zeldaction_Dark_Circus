using BEN.AI;
using UnityEngine;

public class AICustomEffectOnZoneSlow : MonoBehaviour
{
    [SerializeField] private BasicAIBrain _AIBrain;
    [SerializeField, Range(0f, 1f), Tooltip("0f = stun. 1f = no slow")] private float _slowForceMultiplier = 0.5f; 
    [SerializeField, Range(0f, 1f), Tooltip("a value close to zero is great for targets that move very fast")] private float _delayBeforeSlow = 0.25f;

    public void InvokeOnEnter()
    {
        Debug.Log("slowing on enter"); 
        StartCoroutine(nameof(StunMob)); 
    } 
    
    public void InvokeOnExit() 
    {
        Debug.Log("reseting speed on exit");
        ResetInitialValue(); 
    } 

    private System.Collections.IEnumerator StunMob()
    {
        yield return new WaitForSeconds(_delayBeforeSlow); 
        _AIBrain.DefaultSpeed *= _slowForceMultiplier;   
    } 

    private void ResetInitialValue()
    {
        _AIBrain.DefaultSpeed = _AIBrain.InitialSpeed; 
    }

}

