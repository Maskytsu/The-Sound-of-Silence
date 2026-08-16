using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonMonobehaviour<GameManager>
{
    public event Action OnElectricityChange;
    public event Action OnAwareModeChange;
    public bool IsElectricityOn { get; private set; }
    public bool IsAwareModeOn { get; private set; }

    [SerializeField] private SceneSetup _sceneSetup;
    [SerializeField] private SaveManager _saveManager;

    protected override void Awake()
    {
        base.Awake();
        IsElectricityOn = _sceneSetup.IsElectricityOnOnAwake;
        IsAwareModeOn = _sceneSetup.IsAwareModeOnOnAwake;
    }

    public void ChangeElectricityState(bool newState)
    {
        IsElectricityOn = newState;
        OnElectricityChange?.Invoke();
        Debug.Log("Electricity set to: " + newState);
    }

    public void ChangeAwareModeState(bool newState)
    {
        IsAwareModeOn = newState;
        OnAwareModeChange?.Invoke();
        Debug.Log("Aware Mode set to: " + newState);
    }

    public void LoadSceneAndSaveGameState(string scene)
    {
        _saveManager.SaveGameState();
        SceneManager.LoadScene(scene);
    }

    //---------------------------------------------------------
    [Button]
    private void SwapElectricityState()
    {
        ChangeElectricityState(!IsElectricityOn);
    }

    [Button]
    private void SwapAwareModeState()
    {
        ChangeAwareModeState(!IsAwareModeOn);
    }
}
