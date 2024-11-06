using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : Interactable
{
    public event Action OnInteract;

    [DisableIf(nameof(IsApplicationPlaying))]
    [SerializeField] public bool IsTurnedOn;

    [SerializeField] private Transform _switchTransform;
    [SerializeField] private List<GameObject> _lightSources = new();

    private bool IsApplicationPlaying => Application.isPlaying;

    private void Awake()
    {
        UpdateLightsAndSwitch();
    }

    private void OnValidate()
    {
        UpdateLightsAndSwitch();
    }

    public override void Interact()
    {
        IsTurnedOn = !IsTurnedOn;
        UpdateLightsAndSwitch();
        OnInteract?.Invoke();
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
