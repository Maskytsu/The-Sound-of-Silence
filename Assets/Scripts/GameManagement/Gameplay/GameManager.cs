using FMOD.Studio;
using FMODUnity;
using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event Action OnElectricityChange;
    public bool IsElectricityOn { get; private set; }

    [SerializeField] private SceneSetup _sceneSetup;
    [SerializeField] private SaveManager _saveManager;

    private void Awake()
    {
        CreateInstance();
        IsElectricityOn = _sceneSetup.IsElectricityOnOnAwake;
    }

    public void ChangeElectricityState(bool newState)
    {
        IsElectricityOn = newState;
        OnElectricityChange?.Invoke();
    }

    public void LoadSceneAndSaveGameState(string scene)
    {
        _saveManager.SaveGameState();
        SceneManager.LoadScene(scene);
    }

    private void CreateInstance()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one GameManager in the scene.");
        }
        Instance = this;
    }

    //---------------------------------------------------------
    [Button]
    private void SwapElectricityState()
    {
        ChangeElectricityState(!IsElectricityOn);
    }
}
