using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : Interactable
{
    [SerializeField] private Transform _switchTransform;
    [SerializeField] private List<GameObject> _lightSources = new List<GameObject>();
    [SerializeField] private bool _turnedOn = false;

    private void Awake()
    {
        SettLightsAndSwitch();
    }

    public override void Interact()
    {
        TurnOnOffLights();
    }

    private void TurnOnOffLights()
    {
        _turnedOn = !_turnedOn;
        SettLightsAndSwitch();
    }
        private void SettLightsAndSwitch()
    {
        foreach (GameObject lightSource in _lightSources)
        {
            lightSource.SetActive(_turnedOn);
        }

        if (_turnedOn) _switchTransform.localRotation = Quaternion.Euler(-15, 0, 0);
        else _switchTransform.localRotation = Quaternion.Euler(15, 0, 0);
    }
}
