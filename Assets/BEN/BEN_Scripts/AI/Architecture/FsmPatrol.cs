using System;
using UnityEngine;
using UnityEngine.AI;

namespace BEN.AI
{
    [DefaultExecutionOrder(5)]
    public class FsmPatrol : MonoBehaviour
    {
        public Transform patrolZone;  
        public Transform[] Points { get; private set; }
        public int DestPoint { get; private set; }
        private NavMeshAgent _agent;
        
        public bool IsDead { get; set; }

        private void Start() 
        {
            _agent = GetComponent<NavMeshAgent>();

            DestPoint = 0; 
            _agent.autoBraking = true; 
        }

        private void FixedUpdate() 
        { 
            if (IsDead) return;

            if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
                GotoNextPoint();
        } 

        public void SetPoints()
        {
            if (IsDead) return;

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

        /* private void OnTriggerEnter(Collider other)
        {
            if (gameObject.layer == 9 && other.gameObject.layer == 7)
            {
                Debug.Log($"value is {Vector3.Dot(transform.TransformDirection(dete))}");
            }
        } */
    }
}
