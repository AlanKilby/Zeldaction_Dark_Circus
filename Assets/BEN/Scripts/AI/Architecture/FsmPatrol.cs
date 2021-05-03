using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace BEN.AI
{
    [DefaultExecutionOrder(10)]
    public class FsmPatrol : MonoBehaviour
    {
        public Transform patrolZone;  
        public Transform[] Points { get; private set; }
        public int DestPoint { get; private set; }
        private NavMeshAgent _agent;
        [FormerlySerializedAs("_playerDetected")] public bool playerDetected;

        private void Start()
        {
            _agent = GetComponent<NavMeshAgent>();

            DestPoint = 0; 
            _agent.autoBraking = false;

            GotoNextPoint(); 
        }

        private void FixedUpdate() 
        { 
            if (!_agent.pathPending && _agent.remainingDistance < 0.5f && !playerDetected)
                GotoNextPoint();
        }

        public void SetPoints()
        {
            Points = new Transform[patrolZone.childCount]; 
            for (int i = 0; i < patrolZone.childCount; i++)
            {
                Points[i] = patrolZone.GetChild(i); 
            } 
        }

        private void GotoNextPoint() 
        {
            if (Points.Length == 0)
                return;

            _agent.destination = Points[DestPoint].position;
            DestPoint = (DestPoint + 1) % Points.Length; 
        } 
    }
}
