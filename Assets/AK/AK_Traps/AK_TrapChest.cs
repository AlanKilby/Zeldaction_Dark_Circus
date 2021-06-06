using BEN.AI;
using MonsterLove.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_TrapChest : MonoBehaviour
{
    Collider chestCollider;

    [Tooltip("This enemy will be spawned by the trap chest.")]
    public GameObject enemy;

    public Animator destructionAnim;
    public ParticleSystem destructionParticles;

    private void Awake()
    {
        chestCollider = GetComponent<Collider>();
        destructionAnim.Play("caissepiege");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.layer == LayerMask.NameToLayer("PlayerWeapon"))
        {
            chestCollider.enabled = false;
            destructionAnim.Play("crate blowup");
            destructionParticles.Play();
            EnemySpawn();
        }
    }

    public void EnemySpawn()
    {
        GameObject instantiatedEnemy = Instantiate(enemy, transform.position, Quaternion.identity);
        BasicAIBrain aibrain = instantiatedEnemy.GetComponentInChildren<BasicAIBrain>();

        aibrain.HasBeenInvokedByBoss = true;
        aibrain.OnRequireStateChange(States.Attack, StateTransition.Safe);
    } 
}
