using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(10)] 
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;  
    public List<RuntimeAnimatorController> controllersList = new List<RuntimeAnimatorController>();
    public List<AIAnimationSO> scriptableAnimationList = new List<AIAnimationSO>(); 

    private void OnEnable()
    {
        Instance = this; 
    }

}
