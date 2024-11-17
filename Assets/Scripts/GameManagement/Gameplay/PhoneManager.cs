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

    private void Awake()
    {
        CreateInstance();
        ListenToPhoneEvents();

        CurrentPhoneSetup = _sceneSetup.StartingPhoneSetup;
    }

    private void OnDestroy()
    {
        if (CurrentPhoneSetup != null) CurrentPhoneSetup.ClearSubscribersFromContacts();
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

    public void ListenToPhoneEvents()
    {
        if (_claireInteractableContact.IsOnCallNull())
        {
            _claireInteractableContact.OnCall += () => StartCoroutine(CallContact(_claireInteractableContact));
        }
        if (_policeContact.IsOnCallNull())
        {
            _policeContact.OnCall += () => StartCoroutine(CallContact(_policeContact));
        }

        _mechanicContact.OnCheckNew += () => _gameState.MechanicChecked = true;
        _mechanicContact.OnSendMessage += () => _gameState.MechanicMessaged = true;

        _claireInteractableContact.OnSendMessage += () => _gameState.ClaireMessaged = true;
        _claireInteractableContact.OnCall += () => _gameState.ClaireCalled = true;

        _policeContact.OnCheckNew += () => _gameState.PoliceChecked = true;
        _policeContact.OnCall += () => _gameState.PoliceCalled = true;
    }

    private IEnumerator CallContact(ContactScriptable contact)
    {
        ClosePhone();


        if (contact == _claireInteractableContact)
        {
            //--------------------------------------------------------------------------
            EventReference eventRef = FmodEvents.Instance.H_SFX_Calling;
            RuntimeManager.PlayOneShot(eventRef);
            yield return new WaitForSeconds(AudioManager.Instance.EventLength(eventRef));

            _inputProvider.LoadMapStatesAndApplyThem();
        }
        else if (contact == _policeContact)
        {
            if (!_gameState.PoliceCalled)
            {
                //--------------------------------------------------------------------------
                EventReference eventRef = FmodEvents.Instance.H_SFX_Calling;
                EventInstance eventInstance = RuntimeManager.CreateInstance(eventRef);
                eventInstance.start();
                yield return new WaitForSeconds(2.5f);
                eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

                UIManager.Instance.DisplayDialogueSequence(_policeDialogue);
                _policeDialogue.OnDialogueEnd += () =>
                {
                    _inputProvider.LoadMapStatesAndApplyThem();
                    _policeDialogue.OnDialogueEnd = null;
                };
            }
            else
            {
                //--------------------------------------------------------------------------
                UIManager.Instance.DisplayDialogueSequence(_numberUnavailableDialogue);
                _numberUnavailableDialogue.OnDialogueEnd += () =>
                {
                    _inputProvider.LoadMapStatesAndApplyThem();
                    _numberUnavailableDialogue.OnDialogueEnd = null;
                };
            }

        }
        else
        {
            Debug.LogError("Calling not implemented contact!");
            _inputProvider.LoadMapStatesAndApplyThem();
        }
    }

    private void ClosePhone()
    {
        HUD.Instance.MiddlePointer.SetActive(true);
        CameraManager.Instance.PhoneInteractCamera.gameObject.SetActive(false);
        _inputProvider.LockCursor();

        PlayerObjectsHolder.Instance.PlayerEquipment.ChangeItem(ItemType.NONE);
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
