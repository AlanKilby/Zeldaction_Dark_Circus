using UnityEngine;
using MonsterLove.StateMachine; 

public class Switch : MonoBehaviour
{
    [SerializeField] private Color activeSwitchColor;
    [SerializeField] private LayerMask _playerLayer;

    private Light _light;
    private Color _initialColor;
    public bool CanBeDeactivated { get; set; }

    private void Start()
    {
        _light = GetComponent<Light>();
        _initialColor = _light.color; 
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
        if (Mathf.Pow(2f, other.gameObject.layer) == _playerLayer && Input.GetButtonDown("CAC") && CanBeDeactivated) 
        {
            BossAIBrain.sSwitchUsedCount++; 
            CanBeDeactivated = false;  
            _light.color = Color.black;
            Invoke(nameof(ResetState), 0f);

            if (BossAIBrain.sSwitchUsedCount == BossAIBrain.sMaxActiveSwitches)
            { 
                BossAIBrain.OnRequireStateChange(BossStates.Vulnerable, StateTransition.Safe); 
            }
        } 
    } 

    public void ShowSwitchIsOn() 
    { 
        _light.color = activeSwitchColor;
        CanBeDeactivated = true;
        Invoke(nameof(ResetState), BossAIBrain.sLightsOnDuration);  
    }

    private void ResetState() 
    { 
        CanBeDeactivated = false;
        _light.color = _initialColor;  
    }
}
