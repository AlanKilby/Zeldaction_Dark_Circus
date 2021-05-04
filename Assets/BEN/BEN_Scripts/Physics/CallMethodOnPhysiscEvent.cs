using System;
using BEN.Utility;
using BEN.AI; 
using UnityEngine;

public enum Calls { CallOnEnter, CallOnStay, CallOnExit }

namespace BEN
{
    [RequireComponent(typeof(BoxCollider))] // DEBUG
    public class CallMethodOnPhysiscEvent : MonoBehaviour
    {
        [SerializeField, Range(1, 5)] private int radius = 1;
        [SerializeField] private LayerMask detectableTargetsLayer;
        private Collider[] _detectedCollidersArray;
        private bool _playerDetected;
        private RaycastHit[] _detectedColliders;

        private float[] _distances;
        private float _smallestValue;

        private bool _notified; // DEBUG 
        public bool isMascotte = false; 

        public CallMethodOnPhysiscEvent(Collider[] detectedCollidersArray)
        {
            this._detectedCollidersArray = detectedCollidersArray;
        } 

        private void OnDrawGizmos() 
        {
#if UNITY_EDITOR
            UnityEditor.Handles.color = Color.red; 
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, radius); 
#endif
        }

        private void OnTriggerEnter(Collider other) 
        {
        }

        private void OnTriggerStay(Collider other)
        {

        }

        private void OnTriggerExit(Collider other)
        {
            
        }
    }
}
