using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event Action OnElectricityChange;
    public PhoneSetupScriptable CurrentPhoneSetup { get; private set; }
    public bool IsElectricityOn { get; private set; }

    [SerializeField] private SceneSetup _sceneSetup;
    [SerializeField] private SaveManager _saveManager;
    [SerializeField] private GameState _gameState;
    [Space]
    [SerializeField] private ContactScriptable _claireInteractableContact;
    [SerializeField] private ContactScriptable _policeContact;

    private void Awake()
    {
        CreateInstance();
        ListenToPhoneEvents();

        CurrentPhoneSetup = _sceneSetup.StartingPhoneSetup;
        IsElectricityOn = _sceneSetup.IsElectricityOnOnAwake;
    }

    private void OnDestroy()
    {
        if (CurrentPhoneSetup != null) CurrentPhoneSetup.ClearSubscribersFromContacts();
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

    public void ChangePhoneSetup(PhoneSetupScriptable phoneSetup)
    {
        CurrentPhoneSetup.ClearSubscribersFromContacts();
        CurrentPhoneSetup = phoneSetup;

        PhoneScreen phoneScreen = FindObjectOfType<PhoneScreen>();
        if (phoneScreen != null)
        {
            phoneScreen.DisplayContactsMenu();
        }
    }

    public void ListenToPhoneEvents()
    {
        _claireInteractableContact.OnCall += () => StartCoroutine(CallingClaire());
        _policeContact.OnCall += () => StartCoroutine(CallingPolice());
    }

    private IEnumerator CallingClaire()
    {
        yield return null;
    }

    private IEnumerator CallingPolice()
    {
        yield return null;

        if (!_gameState.PoliceCalled)
        {

        }
        else
        {

        }
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
