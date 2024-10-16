using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FmodEvents : MonoBehaviour
{
    [field: Header("Music")]
    [field: SerializeField] public EventReference MUSIC_BackgroundMusicOne { get; private set; }

    [field: Header("SFX Player")]
    [field: SerializeField] public EventReference SFX_PlayerStartedSneaking { get; private set; }
    [field: SerializeField] public EventReference SFX_PlayerFootsteps { get; private set; }

    [field: Header("SFX Environment")]
    [field: SerializeField] public EventReference SFX_TestIdle { get; private set; }
    [field: SerializeField] public EventReference SFX_TestIdleTwo { get; private set; }

    [field: Header("SFX Ambience")]
    [field: SerializeField] public EventReference SFX_Wind { get; private set; }

    public static FmodEvents Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one FmodEvents in the scene.");
        }
        Instance = this;
    }
}
