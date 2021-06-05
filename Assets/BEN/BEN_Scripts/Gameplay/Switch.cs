using UnityEngine;
using MonsterLove.StateMachine; 

public class Switch : MonoBehaviour
{
    [SerializeField] private LayerMask _playerLayer, _playerWeaponLayer;
    public bool CanBeDeactivated { get; set; }
    private GameObject _visualCue; 

    private void Start()
    {
        _visualCue = transform.GetChild(0).gameObject; 
    } 

    private void FixedUpdate() // DEBUG 
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            CanBeDeactivated = false;  
        }
    }

    private void OnTriggerStay(Collider other) 
    {
        if ((Mathf.Pow(2f, other.gameObject.layer) == _playerLayer && Input.GetButtonDown("CAC") 
            || Mathf.Pow(2f, other.gameObject.layer) ==  _playerWeaponLayer) && CanBeDeactivated) 
        {
            BossAIBrain.sSwitchUsedCount++; 
            CanBeDeactivated = false;  
            _visualCue.SetActive(CanBeDeactivated);
            Invoke(nameof(ResetState), 0f);

            if (BossAIBrain.sSwitchUsedCount == BossAIBrain.sMaxActiveSwitches)
            { 
                BossAIBrain.OnRequireStateChange(BossStates.Vulnerable, StateTransition.Safe); 
            }
        } 
    } 

    public void ShowSwitchIsOn() 
    { 
        CanBeDeactivated = true; 
        _visualCue.SetActive(CanBeDeactivated);
        Invoke(nameof(ResetState), BossAIBrain.sLightsOnDuration);  
    }

    private void ResetState() 
    { 
        CanBeDeactivated = false;
        _visualCue.SetActive(CanBeDeactivated);
    }
}
