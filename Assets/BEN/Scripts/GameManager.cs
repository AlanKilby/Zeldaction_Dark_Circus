using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
