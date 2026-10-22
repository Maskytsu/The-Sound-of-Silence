using FMODUnity;
using NaughtyAttributes;
using UnityEngine;

public class AmbientTrigger : Trigger
{
    [SerializeField] private bool _endCurrentAmbient;
    [SerializeField] private bool _startNewAmbient;
    [SerializeField, ShowIf(nameof(_startNewAmbient))] private EventReference _ambient;
    [SerializeField, ShowIf(nameof(_startNewAmbient))] private bool _isAmbient3D;

    protected override void Start()
    {
        base.Start();
        OnObjectTriggerEnter += HandleAmbient;
    }

    private void HandleAmbient()
    {
        gameObject.SetActive(false);

        var ambientManager = MusicAndAmbient.Instance;
        if (_endCurrentAmbient) ambientManager.EndCurrentAmbient();
        if (_startNewAmbient) ambientManager.StartAmbient(_ambient, _isAmbient3D);
    }
}