using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_Potion : MonoBehaviour
{
    Animator potionAnim;
    BoxCollider potionCollider;
    AK_PlayerPotionManager potionManager;
    public float destroyTime = 4;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _audioClip; 
    private void Start()
    {
        potionAnim = GetComponent<Animator>();
        potionCollider = GetComponent<BoxCollider>();
        potionManager = GameObject.FindGameObjectWithTag("Player").GetComponent<AK_PlayerPotionManager>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && AK_PlayerManager.potionNumber < potionManager.maxPotionQuantity)
        {
            _audioSource.PlayOneShot(_audioClip);
            //Debug.Log("PlayerCollisionPotion");
            AK_PlayerManager.potionNumber++;
            potionAnim.Play("collected");
            potionCollider.enabled = false;
            Destroy(gameObject,destroyTime);      
        }
    }

}
