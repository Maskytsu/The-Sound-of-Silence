using FMODUnity;
using System.Collections;
using UnityEngine;

public class CheckMechanicQuestHandler : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private TutorialOverlay _phoneTutorialPrefab;
    [SerializeField] private TutorialOverlay _useItemTutorialPrefab;
    [SerializeField] private TutorialOverlay _freeHandTutorialPrefab;
    [Header("Scriptable Objects")]
    [SerializeField] private QuestScriptable _drinkQuest;
    [SerializeField] private QuestScriptable _checkPhoneQuest;
    [SerializeField] private QuestScriptable _goSleepQuest;
    [SerializeField] private ContactScriptable _mechanicContact;
    [SerializeField] private PhoneSetupScriptable _phoneSetupWithMechanic;

    private TutorialOverlay _phoneTutorial;
    private TutorialOverlay _useItemTutorial;
    private TutorialOverlay _freeHandTutorial;

    private bool _afterUseItemTutorial = false;
    private bool _displayFreeHandTutorial = true;

    private PlayerInputActions.PlayerCameraMapActions PlayerCameraMap => InputProvider.Instance.PlayerCameraMap;
    private PlayerInputActions.UIMapActions UIMap => InputProvider.Instance.UIMap;

    private void Start()
    {
        if (Scene1ResetHandler.Instance.SceneWasReseted)
        {
            PhoneManager.Instance.ChangePhoneSetup(_phoneSetupWithMechanic);
            GameState.Instance.MechanicMessaged = true;
            GameState.Instance.MechanicChecked = true;
            _drinkQuest.OnQuestEnd += () => StartCoroutine(QuestManager.Instance.StartQuestDelayed(_goSleepQuest));
            return;
        }

        _drinkQuest.OnQuestEnd += () => StartCoroutine(QuestManager.Instance.StartQuestDelayed(_checkPhoneQuest));
        _checkPhoneQuest.OnQuestStart += SetupQuest;
        _mechanicContact.OnCheckNew += () => QuestManager.Instance.EndQuest(_checkPhoneQuest);
        _checkPhoneQuest.OnQuestEnd += () => StartCoroutine(QuestManager.Instance.StartQuestDelayed(_goSleepQuest));
    }

    private void Update()
    {
        ManageFreeHandTutorial();
        ManageEndingTutorials();
    }

    private void SetupQuest()
    {
        PhoneManager.Instance.ChangePhoneSetup(_phoneSetupWithMechanic);
        RuntimeManager.PlayOneShot(FmodEvents.Instance.PhoneNewMessage);

        PhoneScreen phoneScreen = FindAnyObjectByType<PhoneScreen>();
        if (phoneScreen != null)
        {
            _useItemTutorial = Instantiate(_useItemTutorialPrefab);
            return;
        }

        _phoneTutorial = Instantiate(_phoneTutorialPrefab);
    }

    private void ManageFreeHandTutorial()
    {
        if (_displayFreeHandTutorial && _afterUseItemTutorial &&
           (UIMap.RightClick.WasPerformedThisFrame() ||
           UIMap.Cancel.WasPerformedThisFrame()))
        {
            _freeHandTutorial = Instantiate(_freeHandTutorialPrefab);
            _displayFreeHandTutorial = false;
        }
    }

    private void ManageEndingTutorials()
    {
        if (_phoneTutorial != null && PlayerCameraMap.GrabItem2.WasPerformedThisFrame())
        {
            _phoneTutorial.EndTutorial();
            _useItemTutorial = Instantiate(_useItemTutorialPrefab);
        }

        if (_useItemTutorial != null && PlayerCameraMap.UseItem.WasPerformedThisFrame())
        {
            _useItemTutorial.EndTutorial();
            _afterUseItemTutorial = true;
        }

        if (_freeHandTutorial != null && PlayerCameraMap.GrabItem1.WasPerformedThisFrame())
        {
            _freeHandTutorial.EndTutorial();
        }
    }
}
