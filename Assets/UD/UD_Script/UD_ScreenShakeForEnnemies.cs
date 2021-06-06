using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UD_ScreenShakeForEnnemies : MonoBehaviour
{
    UD_ScreenShakeScript SS;

    void Start()
    {
        SS = GameObject.FindGameObjectWithTag("Virtual Camera").GetComponent<UD_ScreenShakeScript>();
    }

    public void SmallScreenShake()
    {
        SS.StartSmallShake();
    }
    public void MediumScreenShake()
    {
        SS.StartMediumShake();
    }
    public void HardScreenShake()
    {
        SS.StartHardShake();
    }
}
