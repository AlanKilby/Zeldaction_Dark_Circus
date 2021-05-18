using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class MyEventClass : UnityEvent<float, Transform, Vector3, bool> { } 
public class MascotteCustomEffect : AICustomEffectOnZoneSlow
{
    public MyEventClass myMultiParamsEvents;

    public override void InvokeCustomEvent()
    {
        Debug.Log($"custom effect on {gameObject.name}");
    }
    
    public void AnotherFuncWithAnArguments(float arg, Transform transf, Vector3 pos, bool isValid)
    { 
        
    }  
}
