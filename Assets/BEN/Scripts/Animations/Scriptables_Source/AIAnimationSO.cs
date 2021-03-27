using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AI Animation", menuName = "AI/Animation")]
public class AIAnimationSO : ScriptableObject
{
    public Clips[] clipList; 
}

[System.Serializable]
public class Clips
{
    public string clipName; 
    public AnimationClip clipContainer;
    [Range(0f, 5f)] public float speedMultiplier = 1f; 
} 
