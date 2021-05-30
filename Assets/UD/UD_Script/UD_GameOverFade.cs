using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UD_GameOverFade : MonoBehaviour
{
    public int menuIndex;

    public void LoadMenu()
    {
        SceneManager.LoadScene(menuIndex);
    }
}
