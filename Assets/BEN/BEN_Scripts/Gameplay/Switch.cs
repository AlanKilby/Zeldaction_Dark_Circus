using System;
using BEN.Animation;
using UnityEngine;
using MonsterLove.StateMachine; 

public class Switch : MonoBehaviour
{
    [SerializeField] private LayerMask _playerLayer, _playerWeaponLayer;
    [SerializeField] private AIAnimation _leverAnimation;
    [SerializeField] private AnimEventPlaySound _playSoundOnEvent;
    private UD_BossLeverUI _bossLever;
    public bool CanBeDeactivated { get; set; }
    private GameObject _visualCue;
    
    private void Start()
    {
        _visualCue = transform.GetChild(0).gameObject;
        _leverAnimation.PlayAnimation(AnimState.Idle, AnimDirection.Right);
        _bossLever = GetComponent<UD_BossLeverUI>(); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((Mathf.Pow(2f, other.gameObject.layer) ==  _playerWeaponLayer) && CanBeDeactivated) 
        {
            BossAIBrain.sSwitchUsedCount++; 
            CanBeDeactivated = false;  
            _visualCue.SetActive(CanBeDeactivated);
            Invoke(nameof(ResetState), 0f);
            _leverAnimation.PlayAnimation(AnimState.Idle, AnimDirection.Right); 
            _playSoundOnEvent.PlaySoundSafe(SoundType.Reset);

            if (BossAIBrain.sSwitchUsedCount == BossAIBrain.sMaxActiveSwitches)
            { 
                BossAIBrain.OnRequireStateChange(BossStates.Vulnerable, StateTransition.Safe); 
            } 
        } 
    }

    private void OnTriggerStay(Collider other) 
    {
        if (Mathf.Pow(2f, other.gameObject.layer) == _playerLayer && Input.GetButtonDown("PlayerInteraction")) 
        {
            BossAIBrain.sSwitchUsedCount++; 
            CanBeDeactivated = false;  
            _visualCue.SetActive(CanBeDeactivated);
            Invoke(nameof(ResetState), 0f);
            _leverAnimation.PlayAnimation(AnimState.Idle, AnimDirection.Right); 
            _playSoundOnEvent.PlaySoundSafe(SoundType.Reset);

            if (BossAIBrain.sSwitchUsedCount == BossAIBrain.sMaxActiveSwitches)
            { 
                BossAIBrain.OnRequireStateChange(BossStates.Vulnerable, StateTransition.Safe); 
            } 
        } 
    } 

    public void ShowSwitchIsOn() 
    { 
        CanBeDeactivated = true;
        _leverAnimation.PlayAnimation(AnimState.Idle, AnimDirection.Left); 
        _playSoundOnEvent.PlaySoundSafe(SoundType.Reset); 
        
        _visualCue.SetActive(CanBeDeactivated);
        Invoke(nameof(ResetState), BossAIBrain.sLightsOnDuration);  
    }

    private void ResetState() 
    { 
        CanBeDeactivated = false;
        _bossLever.PlayerIsNear = false; 
        _visualCue.SetActive(CanBeDeactivated);
        _leverAnimation.PlayAnimation(AnimState.Idle, AnimDirection.Right);
    }
}
