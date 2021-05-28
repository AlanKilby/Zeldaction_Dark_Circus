using UnityEngine;
using BEN.AI;
using MonsterLove.StateMachine; 

public class Switch : MonoBehaviour
{
    [SerializeField] private Color activeSwitchColor;
    private Light _light;
    private Color _initialColor;
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
            _light.color = Color.black;
            BossAIBrain.OnRequireStateChange(BossStates.Defend, StateTransition.Overwrite);  
            Invoke(nameof(ResetState), 0f);     
        } 
    } 

    public void ShowIsDeactivatable() 
    { 
        _light.color = activeSwitchColor;
        CanBeDeactivated = true;
        Invoke(nameof(ResetState), 5f);  
    }

    private void ResetState() 
    { 
        CanBeDeactivated = false;
        _light.color = _initialColor;  
    }
}
