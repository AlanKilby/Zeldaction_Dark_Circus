using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UD_EnterDungeonScript : MonoBehaviour
{
    public UD_FadeAndLoadingSceneManager Fade;
    public int dungeonIndex;

    private void OnTriggerEnter(Collider other)
    {
        Fade.nextSceneIndex = dungeonIndex;
        Fade.LaunchFadeIn(dungeonIndex);
    }
}
