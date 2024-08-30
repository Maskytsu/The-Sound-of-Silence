using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StudioEventEmitter))]
public class TestingSoundEmitter : MonoBehaviour
{
    private StudioEventEmitter emitter;

    private void Start()
    {
        emitter = AudioManager._instance.InitializeEventEmitter(FMODEvents.Instance.TestIdle, GetComponent<StudioEventEmitter>());
        emitter.Play();
    }

    private void OnDestroy()
    {
        emitter.Stop();
        emitter.EventInstance.release();
    }
}
