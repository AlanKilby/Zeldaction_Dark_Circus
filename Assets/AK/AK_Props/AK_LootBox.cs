using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_LootBox : MonoBehaviour
{
    Collider chestCollider;

    [HideInInspector]
    public AK_DropRateManager dropRateManager;

    public Animator destructionAnim;
    public ParticleSystem destructionParticles;
    
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _audioClip; 
    
    private void Awake()
    {
        chestCollider = GetComponent<Collider>();
        dropRateManager = GameObject.FindGameObjectWithTag("Player").GetComponent<AK_DropRateManager>();
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.layer == LayerMask.NameToLayer("PlayerWeapon"))
        {
            _audioSource.PlayOneShot(_audioClip); 
            chestCollider.enabled = false;
            destructionAnim.Play("crate blowup");
            destructionParticles.Play();
            dropRateManager.Drop(this.gameObject);
        }
    }

    
}
