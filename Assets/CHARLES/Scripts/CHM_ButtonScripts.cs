using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CHM_ButtonScripts : MonoBehaviour
{
    public int playSceneValue;

    public void Play()
    {
        SceneManager.LoadScene(playSceneValue);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
