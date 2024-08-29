using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference PlayerStartedSneaking { get; private set; }
    [field: SerializeField] public EventReference PlayerFootsteps { get; private set; }

    [field: Header("Environment SFX")]
    [field: SerializeField] public EventReference TestIdle { get; private set; }
    [field: SerializeField] public EventReference TestIdleTwo { get; private set; }

    [field: Header("Ambience")]
    [field: SerializeField] public EventReference Wind { get; private set; }

    [field: Header("Music")]
    [field: SerializeField] public EventReference backgroundMusic1 { get; private set; }

    public static FMODEvents Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one FMOD Events in the scene.");
        }
        Instance = this;
    }
}
