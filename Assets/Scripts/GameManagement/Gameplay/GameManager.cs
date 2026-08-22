using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonMonobehaviour<GameManager>
{
    public event Action OnElectricityChange;
    public bool IsElectricityOn { get; private set; }

    [SerializeField] private SceneSetup _sceneSetup;
    [SerializeField] private SaveManager _saveManager;

    protected override void Awake()
    {
        base.Awake();
        IsElectricityOn = _sceneSetup.IsElectricityOnOnAwake;
    }

    public void ChangeElectricityState(bool newState)
    {
        IsElectricityOn = newState;
        OnElectricityChange?.Invoke();
        Debug.Log("Electricity set to: " + newState);
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
}