using UnityEngine;

[RequireComponent(typeof(ProcessInputs))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private ProcessInputs m_PlayerInputs;
    [SerializeField, Range(2f, 15f)] private float m_MoveSpeed = 8f;

    void FixedUpdate()
    {
        transform.Translate(ProcessInputs.s_Direction.normalized * Time.fixedDeltaTime * m_MoveSpeed, Space.World); 
    }
}
