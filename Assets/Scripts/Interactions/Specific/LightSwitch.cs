using FMODUnity;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : Interactable
{
    [Space]
    [DisableIf(nameof(IsApplicationPlaying))]
    public bool IsTurnedOn;

    [SerializeField] private Transform _switchTransform;
    [SerializeField] private List<GameObject> _lightSources = new();

    private bool IsApplicationPlaying => Application.isPlaying;

    private void Start()
    {
        UpdateLights();
        UpdateSwitch();
        GameManager.Instance.OnElectricityChange += UpdateLights;
    }

    private void OnValidate()
    {
        UpdateLights();
        UpdateSwitch();
    }

    protected override void Interact()
    {
        IsTurnedOn = !IsTurnedOn;
        RuntimeManager.PlayOneShotAttached(FmodEvents.Instance.SPT_LightSwitchClick, _switchTransform.gameObject);
        UpdateLights();
        UpdateSwitch();
    }

    private void UpdateLights()
    {
        bool lightsState;

        if (GameManager.Instance != null && !GameManager.Instance.IsElectricityOn)
        {
            lightsState = false;
        }
        else
        {
            lightsState = IsTurnedOn;
        }

        foreach (GameObject lightSource in _lightSources)
        {
            lightSource.SetActive(lightsState);
        }
    }

    private void UpdateSwitch()
    {
        if (IsTurnedOn) _switchTransform.localRotation = Quaternion.Euler(-15, 0, 0);
        else _switchTransform.localRotation = Quaternion.Euler(15, 0, 0);
    }
}