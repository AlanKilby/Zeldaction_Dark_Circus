using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UD_UIPauseMenuExit : MonoBehaviour
{
    public UD_FadeAndLoadingSceneManager Fade;
    public int MenuIndex;

    public CHM_Pause pauseScript;
    public void ReturnMenu()
    {
        pauseScript.ChangePauseState();
        Fade.ChangeAnimationState(Fade.Fade_In);
        Fade.nextSceneIndex = MenuIndex;
    }
}
