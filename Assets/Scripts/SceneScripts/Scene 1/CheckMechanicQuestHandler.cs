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
    [SerializeField] private ContactScriptable _mechanicContact;
    [SerializeField] private PhoneSetupScriptable _phoneSetupWithMechanic;

    private GameObject _phoneTutorial;
    private GameObject _useItemTutorial;
    private GameObject _freeHandTutorial;

    private bool _afterUseItemTutorial = false;
    private bool _displayFreeHandTutorial = true;

    private PlayerInputActions.PlayerMainMapActions PlayerMainMap => InputProvider.Instance.PlayerMainMap;
    private PlayerInputActions.UICustomMapActions UICustomMap => InputProvider.Instance.UICustomMap;

    private void Start()
    {
        _drinkQuest.OnQuestEnd += () => StartCoroutine(StartPhoneQuestDelayed(2f));

        _checkPhoneQuest.OnQuestStart += SetupQuest;

        _mechanicContact.OnCheckNew += () => QuestManager.Instance.EndQuest(_checkPhoneQuest);
    }

    private void Update()
    {
        ManageFreeHandTutorial();
        ManageDestroyingTutorials();
    }

    private IEnumerator StartPhoneQuestDelayed(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        QuestManager.Instance.StartQuest(_checkPhoneQuest);
    }

    private void SetupQuest()
    {
        GameManager.Instance.ChangePhoneSetup(_phoneSetupWithMechanic);

        PhoneScreen phoneScreen = FindObjectOfType<PhoneScreen>();

        if (phoneScreen != null)
        {
            phoneScreen.DisplayContactsMenu();
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
           (UICustomMap.RightClick.WasPerformedThisFrame() ||
           UICustomMap.Cancel.WasPerformedThisFrame()))
        {
            _freeHandTutorial = Instantiate(_freeHandTutorialPrefab);
            _displayFreeHandTutorial = false;
        }
    }

    private void ManageDestroyingTutorials()
    {
        if (_phoneTutorial != null && PlayerMainMap.GrabItem2.WasPerformedThisFrame())
        {
            Destroy(_phoneTutorial);
            _useItemTutorial = Instantiate(_useItemTutorialPrefab);
        }

        if (_useItemTutorial != null && PlayerMainMap.UseItem.WasPerformedThisFrame())
        {
            Destroy(_useItemTutorial);
            _afterUseItemTutorial = true;
        }

        if (_freeHandTutorial != null && PlayerMainMap.GrabItem1.WasPerformedThisFrame())
        {
            Destroy(_freeHandTutorial);
        }
    }
}
