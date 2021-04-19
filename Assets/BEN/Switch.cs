using UnityEngine;
using BEN.Scripts.FSM;
using MonsterLove.StateMachine; 

public class Switch : MonoBehaviour
{
    [SerializeField] private Color activeSwitchColor;
    private Light _light;
    private Color _initialColor;
    public bool IsOff { get; set; }
    public bool CanBeDeactivated { get; set; }

    private void Start()
    {
        _light = GetComponent<Light>();
        _initialColor = _light.color; 
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.Space) && CanBeDeactivated) 
        {
            Debug.Log("PlayerIsDetected");
            CanBeDeactivated = false;  
            IsOff = true;
            _light.color = Color.black;
            BossAIBrain.OnRequireStateChange(States.Defend, StateTransition.Overwrite);  
            Invoke(nameof(ResetState), BossAIBrain.sBossVulnerabilityDuration);    
        } 
    } 

    public void ShowIsDeactivatable() 
    { 
        _light.color = activeSwitchColor;
        CanBeDeactivated = true; 
    }

    private void ResetState() 
    { 
        CanBeDeactivated = false;
        IsOff = false;
        _light.color = _initialColor;  
    }
}
