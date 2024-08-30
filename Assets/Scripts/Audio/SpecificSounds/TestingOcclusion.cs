using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Occlusion))]
public class TestingOcclusion : MonoBehaviour
{
    private EventReference selectedAudio;
    private EventInstance audioEvent;
    private Occlusion occlusion;
    void Start()
    {
        occlusion = GetComponent<Occlusion>();
        selectedAudio = FmodEvents.Instance.SFX_TestIdleTwo;
        occlusion.selectedAudio = selectedAudio;

        occlusion.audioEvent = audioEvent = AudioManager.instance.CreateEventInstance(selectedAudio);
        RuntimeManager.AttachInstanceToGameObject(audioEvent, transform);

        audioEvent.start();
        audioEvent.release();
    }

    private void OnEnable()
    {
        audioEvent.setPaused(false);
    }
    private void OnDisable()
    {
        audioEvent.setPaused(true);
    }
    private void OnDestroy()
    {
        audioEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}
