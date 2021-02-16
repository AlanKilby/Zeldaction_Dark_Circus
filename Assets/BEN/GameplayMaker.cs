using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(ProcessInputs))]
public class GameplayMaker : MonoBehaviour
{
    #region variables 

    private ProcessInputs m_PlayerInputs;
    public static SphereCollider s_RangeDetection;

    [Header("Gameplay Type")]
    [SerializeField] private bool m_Cac;
    [SerializeField] private bool m_Distance;
    [SerializeField] private bool m_Defense;
    [SerializeField] private bool m_Grapling;

    private bool
        cacLoaded,
        distanceLoaded,
        defenseLoaded,
        graplingLoaded; 

    public static bool s_bCac;
    public static bool s_bDistance;
    public static bool s_bDefense; 
    public static bool s_bGrapling; 

    public static Gameplay
        s_GameplayActionCAC,
        s_GameplayActionDistance,
        s_GameplayActionDefense,
        s_GameplayActionGrapling;

    private Vector3 newPosition; 

    #endregion

    #region methods

    private void Start()
    {
        s_RangeDetection = GetComponent<SphereCollider>();
        m_PlayerInputs = GetComponent<ProcessInputs>(); 
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            s_bCac = m_Cac;
            s_bDistance = m_Distance;
            s_bDefense = m_Defense;
            s_bGrapling = m_Grapling;

            if (m_Cac && !s_GameplayActionCAC && !cacLoaded)
            {
                cacLoaded = true;
                s_GameplayActionCAC = gameObject.AddComponent<CAC>();
            }
            else if (!m_Cac && s_GameplayActionCAC)
                StartCoroutine(RemoveFromEditorView(s_GameplayActionCAC, cacLoaded));

            if (m_Distance && !s_GameplayActionDistance && !distanceLoaded)
            {
                distanceLoaded = true;
                s_GameplayActionDistance = gameObject.AddComponent<Distance>();
            }
            else if (!m_Distance && s_GameplayActionDistance)
                StartCoroutine(RemoveFromEditorView(s_GameplayActionDistance, distanceLoaded));

            if (m_Defense && !s_GameplayActionDefense && !defenseLoaded)
            {
                defenseLoaded = true;
                s_GameplayActionDefense = gameObject.AddComponent<Defense>();
            }
            else if (!m_Defense && s_GameplayActionDefense)
                StartCoroutine(RemoveFromEditorView(s_GameplayActionDefense, defenseLoaded));

            if (m_Grapling && !s_GameplayActionGrapling && !graplingLoaded)
            {
                graplingLoaded = true;
                s_GameplayActionGrapling = gameObject.AddComponent<Grapling>();
            }
            else if (!m_Grapling && s_GameplayActionGrapling)
                StartCoroutine(RemoveFromEditorView(s_GameplayActionGrapling, graplingLoaded));

            cacLoaded = m_Cac;
            distanceLoaded = m_Distance;
            defenseLoaded = m_Defense;
            graplingLoaded = m_Grapling;
        }
        else // because problems in editor mode (value reset to False during a frame ?)
        {
            m_Cac = false;
            m_Distance = false;
            m_Defense = false;
            m_Grapling = false;
        }
    }

    private void Update() 
    {
        if (m_PlayerInputs.CacButtonPressed && m_Cac)
        {
            s_GameplayActionCAC.DoAction(); 
        }
        else if (m_PlayerInputs.DefenseButtonPressed && m_Defense)
        {
            s_GameplayActionDefense.DoAction(); 
        }
        else if (m_PlayerInputs.DistanceButtonPressed && m_Distance)
        {
            s_GameplayActionDistance.DoAction(); 
        }
        else if (m_PlayerInputs.GraplingButtonPressed && m_Grapling) 
        {
            s_GameplayActionGrapling.DoAction(); 
        }
        else if (s_RangeDetection.radius > 0f && !m_PlayerInputs.GameplayButtonPressed)
        {
            Invoke(nameof(ResetSphereRadius), 0.5f); 
        }
    }

    #endregion

    #region functions

    private IEnumerator RemoveFromEditorView(Gameplay gameplayElement, bool gameplayActionIsLoaded)
    {
        yield return null;
        gameplayActionIsLoaded = false; 
        DestroyImmediate(gameplayElement);
    }

    private void ResetSphereRadius()
    {
        s_RangeDetection.radius = 0f; 
    }

    #endregion
}
