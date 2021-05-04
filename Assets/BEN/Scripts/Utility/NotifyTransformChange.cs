using System;
using BEN.AI;
using UnityEngine;


namespace BEN.Utility
{
    [ExecuteInEditMode]
    public class NotifyTransformChange : MonoBehaviour
    {
        [SerializeField] private FsmPatrol _patrol; 
        private void OnTransformChildrenChanged()
        {
            _patrol.SetPoints();
        } 
    }
}

