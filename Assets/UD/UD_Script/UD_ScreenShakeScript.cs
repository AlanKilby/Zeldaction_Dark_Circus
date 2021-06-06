using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;

public class UD_ScreenShakeScript : MonoBehaviour
{
    [Header("Small Shake")]
    public float smallShakeDuration;
    public float smallShakeAmplitude;
    public float smallShakeFrequency;
    
    [Header("Medium Shake")]
    public float mediumShakeDuration;
    public float mediumShakeAmplitude;
    public float mediumShakeFrequency;

    [Header("Hard Shake")]
    public float hardShakeDuration;
    public float hardShakeAmplitude;
    public float hardShakeFrequency;

    float currentShakeAmplitude;
    float currentShakeFrequency;

    private float ShakeElapsedTime = 0f;

    // Cinemachine Shake
    public CinemachineVirtualCamera VirtualCamera;
    private CinemachineBasicMultiChannelPerlin virtualCameraNoise;

    void Start()
    {
        if (VirtualCamera != null)
            virtualCameraNoise = VirtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
    }

    void Update()
    {
        // TODO: Replace with your trigger
        if (Input.GetKey(KeyCode.W))
        {
            StartSmallShake();
        }
        if (Input.GetKey(KeyCode.X))
        {
            StartMediumShake();
        }
        if (Input.GetKey(KeyCode.C))
        {
            StartHardShake();
        }

        // If the Cinemachine componet is not set, avoid update
        if (VirtualCamera != null && virtualCameraNoise != null)
        {
            // If Camera Shake effect is still playing
            if (ShakeElapsedTime > 0)
            {
                // Set Cinemachine Camera Noise parameters
                virtualCameraNoise.m_AmplitudeGain = currentShakeAmplitude;
                virtualCameraNoise.m_FrequencyGain = currentShakeFrequency;

                // Update Shake Timer
                ShakeElapsedTime -= Time.deltaTime;
            }
            else
            {
                // If Camera Shake effect is over, reset variables
                virtualCameraNoise.m_AmplitudeGain = 0f;
                ShakeElapsedTime = 0f;
            }
        }
    }

    public void StartSmallShake()
    {
        ShakeElapsedTime = smallShakeDuration;
        currentShakeAmplitude = smallShakeAmplitude;
        currentShakeFrequency = smallShakeFrequency;
    }
    
    public void StartMediumShake()
    {
        ShakeElapsedTime = mediumShakeDuration;
        currentShakeAmplitude = mediumShakeAmplitude;
        currentShakeFrequency = mediumShakeFrequency;
    }
    
    public void StartHardShake()
    {
        ShakeElapsedTime = hardShakeDuration;
        currentShakeAmplitude = hardShakeAmplitude;
        currentShakeFrequency = hardShakeFrequency;
    }
}
