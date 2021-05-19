using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHM_Pause : MonoBehaviour
{
    public GameObject pauseObject;

    private bool gameIsPaused;

    private void Start()
    {
        gameIsPaused = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameIsPaused = !gameIsPaused;
            PauseGame();
        }
    }

    void PauseGame()
    {
        if (gameIsPaused)
        {
            pauseObject.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            pauseObject.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void ChangePauseState()
    {
        gameIsPaused = !gameIsPaused;
        PauseGame();
    }
}
