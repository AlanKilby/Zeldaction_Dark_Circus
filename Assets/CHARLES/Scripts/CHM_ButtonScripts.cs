using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CHM_ButtonScripts : MonoBehaviour
{
    public int playSceneValue;

    public UD_FadeAndLoadingSceneManager fade;

    public void Play()
    {
        fade.LaunchFadeIn(playSceneValue);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
