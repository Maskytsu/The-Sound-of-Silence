using UnityEngine;
using FMODUnity;
using NaughtyAttributes;

public class FmodEvents : MonoBehaviour
{
    public static FmodEvents Instance { get; private set; }

    [field: Header("SFX")]
    [field: SerializeField, BoxGroup("Hearing")] public EventReference H_SFX_PlayerStartedSneaking { get; private set; }
    [field: SerializeField, BoxGroup("Hearing")] public EventReference H_SFX_PlayerFootsteps { get; private set; }
    [field: SerializeField, BoxGroup("Hearing")] public EventReference H_SFX_NoiseSPAT { get; private set; }
    [field: SerializeField, BoxGroup("Hearing")] public EventReference H_SFX_RockRadioOCC { get; private set; }
    [field: SerializeField, BoxGroup("Hearing")] public EventReference H_SFX_RandomSoundOCC { get; private set; }
    [field: SerializeField, BoxGroup("Hearing")] public EventReference H_SFX_Wind { get; private set; }
    [field: Header("Music")]
    [field: SerializeField, BoxGroup("Hearing")] public EventReference H_MUSIC_BackgroundMusicOne { get; private set; }

    [field: Header("SFX")]
    [field: SerializeField, BoxGroup("Silence")] public EventReference S_SFX_Noise { get; private set; }
    [field: SerializeField, BoxGroup("Silence")] public EventReference S_SFX_RandomSoundOCC { get; private set; }

    private void Awake()
    {
        CreateInstance();
    }

    private void CreateInstance()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one FmodEvents in the scene.");
        }
        Instance = this;
    }
}
