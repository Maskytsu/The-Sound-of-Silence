using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference playerStartedSneaking { get; private set; }
    [field: SerializeField] public EventReference playerFootsteps { get; private set; }

    [field: Header("Environment SFX")]
    [field: SerializeField] public EventReference testIdle { get; private set; }
    [field: SerializeField] public EventReference testIdleTwo { get; private set; }

    [field: Header("Ambience")]
    [field: SerializeField] public EventReference wind { get; private set; }

    [field: Header("Music")]
    [field: SerializeField] public EventReference backgroundMusic1 { get; private set; }

    public static FMODEvents instance { get; private set; }
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one FMOD Events in the scene.");
        }
        instance = this;
    }
}
