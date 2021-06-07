using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UD_FadeAndLoadingSceneManager : MonoBehaviour
{
    public int nextSceneIndex;
    public int deathSceneIndex;

    public Animator anim;


    public string currentState;

    public string Fade_Out = "fade out";
    public string Fade_In = "fade in";
    public string Loyal_Fade_In = "loyal fade in";

    private void Start()
    {
        ChangeAnimationState(Fade_Out);
    }

    public void LaunchDeathFade()
    {
        nextSceneIndex = deathSceneIndex;
        ChangeAnimationState(Loyal_Fade_In);
    }

    public void LaunchFadeIn(int nextScene)
    {
        nextSceneIndex = nextScene;
        ChangeAnimationState(Fade_In);
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene(nextSceneIndex);
    }
    
    public void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;

        anim.Play(newState);
        currentState = newState;
    }
}
