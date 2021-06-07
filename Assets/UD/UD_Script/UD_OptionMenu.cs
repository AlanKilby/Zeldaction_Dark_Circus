using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public enum Difficulty { Easy, Normal, Hard}
public class UD_OptionMenu : MonoBehaviour
{
    public Dropdown resolutionDropdown;
    public Slider volumeSlider;

    Resolution[] resolutions;
    public DifficultySettings _difficultySettings; 

    void Start()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();


        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    private void Update()
    {
        //static.volume = volumeSlider.value;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetDifficulty(int DifficultyIndex)
    {
        _difficultySettings.Value = (Difficulty)DifficultyIndex; 
        /* if (DifficultyIndex == 0)
        {
            SetEasy();
        }
        if (DifficultyIndex == 1)
        {
            SetNormal();
        }
        if (DifficultyIndex == 2)
        {
            SetHard();
        } */
    }


    public void SetEasy()
    {
        print("easy");
    }
    
    public void SetNormal()
    {
        print("normal");
        //currentDifficulty = Difficulty.Normal;
    }
    
    public void SetHard()
    {
        print("hard");
        //currentDifficulty = Difficulty.Hard;
    }
}
