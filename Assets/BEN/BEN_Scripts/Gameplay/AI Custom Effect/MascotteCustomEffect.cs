using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class MyEventClass : UnityEvent<float, Transform, Vector3, bool> { } 
public class MascotteCustomEffect : AICustomEffectOnZoneSlow
{
    public MyEventClass myMultiParamsEvents;

    public override void InvokeCustomEventOnEnter()
    {
        Debug.Log($"custom effect on {gameObject.name}");
    }
    
    public override void InvokeCustomEventOnExit()
    {
        Debug.Log($"custom effect exit on {gameObject.name}");
    }
    
    public void FuncWithManyArguments(float arg, Transform transf, Vector3 pos, bool isValid)
    { 
        
    }  
}
