using System.Collections;
using UnityEngine;

public class CheckMechanicQuestHandler : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject _phoneTutorialPrefab;
    [SerializeField] private GameObject _useItemTutorialPrefab;
    [SerializeField] private GameObject _freeHandTutorialPrefab;
    [Header("Scriptable Objects")]
    [SerializeField] private QuestScriptable _drinkQuest;
    [SerializeField] private QuestScriptable _checkPhoneQuest;
    [SerializeField] private QuestScriptable _goSleepQuest;
    [SerializeField] private ContactScriptable _mechanicContact;
    [SerializeField] private PhoneSetupScriptable _phoneSetupWithMechanic;

    private GameObject _phoneTutorial;
    private GameObject _useItemTutorial;
    private GameObject _freeHandTutorial;

    private bool _afterUseItemTutorial = false;
    private bool _displayFreeHandTutorial = true;

    private PlayerInputActions.PlayerCameraMapActions PlayerCameraMap => InputProvider.Instance.PlayerCameraMap;
    private PlayerInputActions.UIMapActions UIMap => InputProvider.Instance.UIMap;

    private void Start()
    {
        _drinkQuest.OnQuestEnd += () => StartCoroutine(QuestManager.Instance.StartQuestDelayed(_checkPhoneQuest));

        _checkPhoneQuest.OnQuestStart += SetupQuest;

        _mechanicContact.OnCheckNew += () => QuestManager.Instance.EndQuest(_checkPhoneQuest);

        _checkPhoneQuest.OnQuestEnd += () => StartCoroutine(QuestManager.Instance.StartQuestDelayed(_goSleepQuest));
    }

    private void Update()
    {
        ManageFreeHandTutorial();
        ManageDestroyingTutorials();
    }

    private void SetupQuest()
    {
        PhoneManager.Instance.ChangePhoneSetup(_phoneSetupWithMechanic);

        PhoneScreen phoneScreen = FindObjectOfType<PhoneScreen>();
        if (phoneScreen != null)
        {
            _useItemTutorial = Instantiate(_useItemTutorialPrefab);
            return;
        }

        DisplayPhoneTutorial();
    }

    private void DisplayPhoneTutorial()
    {
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

    private void ManageDestroyingTutorials()
    {
        if (_phoneTutorial != null && PlayerCameraMap.GrabItem2.WasPerformedThisFrame())
        {
            Destroy(_phoneTutorial);
            _useItemTutorial = Instantiate(_useItemTutorialPrefab);
        }

        if (_useItemTutorial != null && PlayerCameraMap.UseItem.WasPerformedThisFrame())
        {
            Destroy(_useItemTutorial);
            _afterUseItemTutorial = true;
        }

        if (_freeHandTutorial != null && PlayerCameraMap.GrabItem1.WasPerformedThisFrame())
        {
            Destroy(_freeHandTutorial);
        }
    }
}
