using UnityEngine;
using System.Collections.Generic;
using BEN.Animation; 

[CreateAssetMenu(fileName = "New AI Animation", menuName = "AI/Animation")]
public class AIAnimationSO : ScriptableObject
{
    public RuntimeAnimatorController controller; 
    public Dictionary<AnimState, Clips[]> clipListDictionary; // serialize this 

    public void PopulateDictionary()
    {
        
    }
} 

[System.Serializable] 
public class Clips 
{
    public AnimState clipType;
    public AnimState clipDirection; // to avoid OutOfRangeException 
    public AnimationClip clipContainer;
    [Range(0f, 5f)] public float speedMultiplier = 1f; 
} 
