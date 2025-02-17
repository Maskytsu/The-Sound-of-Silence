using FMOD.Studio;
using FMODUnity;
using System.Collections;
using UnityEngine;

public class PhoneManager : MonoBehaviour
{
    public static PhoneManager Instance { get; private set; }

    public PhoneSetupScriptable CurrentPhoneSetup { get; private set; }

    [SerializeField] private SceneSetup _sceneSetup;
    [SerializeField] private GameState _gameState;
    [SerializeField] private InputProvider _inputProvider;
    [Space]
    [SerializeField] private ContactScriptable _mechanicContact;
    [SerializeField] private ContactScriptable _claireInteractableContact;
    [SerializeField] private ContactScriptable _policeContact;
    [Space]
    [SerializeField] private DialogueSequenceScriptable _policeDialogue;
    [SerializeField] private DialogueSequenceScriptable _numberUnavailableDialogue;
    [SerializeField] private DialogueSequenceScriptable _phoneSaysUnavailableDialogue;
    [SerializeField] private DialogueSequenceScriptable _numberNotAnsweringDialogue;
    [SerializeField] private DialogueSequenceScriptable _phoneSaysNotAnsweringDialogue;

    private void Awake()
    {
        CreateInstance();
        ListenToPhoneEvents();

        CurrentPhoneSetup = _sceneSetup.StartingPhoneSetup;
    }

    private void OnDestroy()
    {
        if (CurrentPhoneSetup != null) CurrentPhoneSetup.ClearSubscribersFromContacts();

        //it needs to be here to clear OnCall events which may not be in the CurrentPhoneSetup
        //the OnCall events are assigned even if contacts are not in CurrentPhoneSetup
        //without it the assigned events would stay between scenes and crushed
        _claireInteractableContact.ClearSubscribers();
        _policeContact.ClearSubscribers();
    }

    public void ChangePhoneSetup(PhoneSetupScriptable phoneSetup)
    {
        CurrentPhoneSetup.ClearSubscribersFromContacts();
        ListenToPhoneEvents();
        CurrentPhoneSetup = phoneSetup;

        PhoneScreen phoneScreen = FindObjectOfType<PhoneScreen>();
        if (phoneScreen != null)
        {
            phoneScreen.DisplayContactsMenu();
        }
    }

    private void ListenToPhoneEvents()
    {
        //those checks prevents assigning events twice when changing CurrentPhoneSetup from one that doesn't have those contacts to one that has them
        //this would be assigne twice, because events from those contacts wouldn't be cleared while changing setup
        if (_claireInteractableContact.IsOnCallNull())
        {
            _claireInteractableContact.OnCall += () => StartCoroutine(CallClaireContact());
        }
        if (_policeContact.IsOnCallNull())
        {
            _policeContact.OnCall += () => StartCoroutine(CallPoliceContact());
        }

        _mechanicContact.OnCheckNew += () => _gameState.MechanicChecked = true;
        _mechanicContact.OnSendMessage += () => _gameState.MechanicMessaged = true;

        _claireInteractableContact.OnSendMessage += () => _gameState.ClaireMessaged = true;
        _claireInteractableContact.OnCall += () => _gameState.ClaireCalled = true;

        _policeContact.OnCheckNew += () => _gameState.PoliceChecked = true;
        _policeContact.OnCall += () => _gameState.PoliceCalled = true;
    }

    private IEnumerator CallClaireContact()
    {
        ClosePhone();

        if (AudioManager.Instance.IsAbleToHear)
        {
            RuntimeManager.PlayOneShot(FmodEvents.Instance.PhoneCalling);
            yield return new WaitForSeconds(AudioManager.Instance.EventLength(FmodEvents.Instance.PhoneCalling));
            DisplayPhoneDialogue(_numberNotAnsweringDialogue);
        }
        else
        {
            yield return new WaitForSeconds(2f);
            DisplayPhoneDialogue(_phoneSaysNotAnsweringDialogue);
        }
    }

    private IEnumerator CallPoliceContact()
    {
        ClosePhone();

        if (!_gameState.PoliceCalled && AudioManager.Instance.IsAbleToHear)
        {
            EventInstance eventInstance = AudioManager.Instance.PlayOneShotRI(FmodEvents.Instance.PhoneCalling);
            yield return new WaitForSeconds(2.5f);
            eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

            DisplayPhoneDialogue(_policeDialogue);
        }
        else if (AudioManager.Instance.IsAbleToHear)
        {
            yield return new WaitForSeconds(0.5f);
            DisplayPhoneDialogue(_numberUnavailableDialogue);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            DisplayPhoneDialogue(_phoneSaysUnavailableDialogue);
        }
    }

    private void DisplayPhoneDialogue(DialogueSequenceScriptable dialogue)
    {
        UIManager.Instance.DisplayDialogueSequence(dialogue);
        dialogue.OnDialogueEnd += () =>
        {
            _inputProvider.LoadMapStatesAndApplyThem();
            dialogue.OnDialogueEnd = null;
        };
    }

    private void ClosePhone()
    {
        HUD.Instance.MiddlePointer.SetActive(true);
        CameraManager.Instance.PhoneInteractCamera.gameObject.SetActive(false);
        _inputProvider.LockCursor();

        PlayerObjects.Instance.PlayerEquipment.ChangeItem(ItemType.NONE);
    }

    private void CreateInstance()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one PhoneManager in the scene.");
        }
        Instance = this;
    }
}