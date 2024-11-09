using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : Interactable
{
    [Space]
    [DisableIf(nameof(IsApplicationPlaying))]
    [SerializeField] public bool IsTurnedOn;

    [SerializeField] private Transform _switchTransform;
    [SerializeField] private List<GameObject> _lightSources = new();

    private bool IsApplicationPlaying => Application.isPlaying;

    protected override void Awake()
    {
        UpdateLightsAndSwitch();
        AssignMethodsToEvents();
    }

    private void OnValidate()
    {
        UpdateLightsAndSwitch();
    }

    protected override void Interact()
    {
        IsTurnedOn = !IsTurnedOn;
        UpdateLightsAndSwitch();
    }

    private void UpdateLightsAndSwitch()
    {
        foreach (GameObject lightSource in _lightSources)
        {
            lightSource.SetActive(IsTurnedOn);
        }

        if (IsTurnedOn) _switchTransform.localRotation = Quaternion.Euler(-15, 0, 0);
        else _switchTransform.localRotation = Quaternion.Euler(15, 0, 0);
    }
}