using UnityEngine;

[System.Serializable]
public abstract class Gameplay : MonoBehaviour
{
    protected Collider[] targetDetected; 

    public abstract void DoAction();

    public abstract float GetRange();

    public abstract Color GetColor();
}
