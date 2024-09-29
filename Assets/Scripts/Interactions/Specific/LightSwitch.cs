using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : Interactable
{
    public Action OnInteract;
    [field: SerializeField] public bool IsTurnedOn { get; private set; }

    [SerializeField] private Transform _switchTransform;
    [SerializeField] private List<GameObject> _lightSources = new();

    private void Awake()
    {
        SettLightsAndSwitch();
    }

    public override void Interact()
    {
        TurnOnOffLights();
        OnInteract?.Invoke();
    }

    private void TurnOnOffLights()
    {
        IsTurnedOn = !IsTurnedOn;
        SettLightsAndSwitch();
    }

    private void SettLightsAndSwitch()
    {
        foreach (GameObject lightSource in _lightSources)
        {
            lightSource.SetActive(IsTurnedOn);
        }

        if (IsTurnedOn) _switchTransform.localRotation = Quaternion.Euler(-15, 0, 0);
        else _switchTransform.localRotation = Quaternion.Euler(15, 0, 0);
    }
}
