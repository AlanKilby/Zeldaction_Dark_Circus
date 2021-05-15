using UnityEngine;
using System.Collections.Generic;
using BEN.Animation; 
using Sirenix.OdinInspector; 

[CreateAssetMenu(fileName = "New AI Animation", menuName = "AI/Animation")]
public class AIAnimationSO : SerializedScriptableObject
{
    public RuntimeAnimatorController controller;
    public Dictionary<AnimState, Clip[]> clipListDictionary = new Dictionary<AnimState, Clip[]>();

    public Clip GetAnimClipFromDictionary(AnimState key, AnimDirection direction)
    {
        for (var i = 0; i < clipListDictionary[key].Length; i++)
        {
            if (direction == clipListDictionary[key][i].clipDirection)
            {
                return clipListDictionary[key][i]; 
            } 
        }
        return null; 
    }
} 

[System.Serializable] 
public class Clip 
{
    public AnimDirection clipDirection; // to avoid OutOfRangeException 
    public AnimationClip clipContainer;
    [Range(0f, 5f)] public float speedMultiplier = 1f; 
} 


