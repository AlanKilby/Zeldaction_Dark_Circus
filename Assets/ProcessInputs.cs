using UnityEngine;

public class ProcessInputs : MonoBehaviour
{
    public static Vector3 s_Direction; 
    public bool CacButtonPressed { get; private set; }
    public bool DistanceButtonPressed { get; private set; } 
    public bool DefenseButtonPressed { get; private set; }
    public bool GraplingButtonPressed { get; private set; }
    public bool GameplayButtonPressed { get; private set; }

    void Update()
    {
        s_Direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        CacButtonPressed = Input.GetButtonDown("CAC");
        DistanceButtonPressed = Input.GetButtonDown("Distance");
        DefenseButtonPressed = Input.GetButtonDown("Defense");
        GraplingButtonPressed = Input.GetButtonDown("Grapling");
        GameplayButtonPressed = CacButtonPressed ^ DistanceButtonPressed ^ DefenseButtonPressed ^ GraplingButtonPressed;
    }
}
