using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_TimedDestruction : MonoBehaviour
{
    public float destroyTimer;
    private void Start()
    {
        Destroy(gameObject, destroyTimer);
    }
}
