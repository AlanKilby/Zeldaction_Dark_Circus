using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UD_WinscreenFade : MonoBehaviour
{
    Animator anim;

    int sceneToLoad;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void SetActiveFalse()
    {
        gameObject.SetActive(false);
    }

    public void LoadNextScene(int nextScene)
    {
        anim.Play("WinscreenFadeOut");
        sceneToLoad = nextScene;
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
