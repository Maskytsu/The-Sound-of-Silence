using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPhoneQuestHandler : MonoBehaviour
{
    [Header("Tutorial Prefabs")]
    [SerializeField] private GameObject _phoneTutorialPrefab;
    [SerializeField] private GameObject _useItemTutorialPrefab;
    [Header("Needed Scriptables")]
    [SerializeField] private QuestScriptable _checkPhoneQuest;
    [SerializeField] private ContactScriptable _mechanicContact;
    [SerializeField] private PhoneSetupScriptable _phoneSetupWithMechanic;

    private GameObject _phoneTutorial;
    private GameObject _useItemTutorial;

    private PlayerInputActions.PlayerKeyboardMapActions PlayerKeyboardMap => InputProvider.Instance.PlayerKeyboardMap;
    private PlayerInputActions.PlayerMouseMapActions PlayerMouseMap => InputProvider.Instance.PlayerMouseMap;

    private void Start()
    {
        _checkPhoneQuest.OnQuestStart += ChangePhoneSetup;
        _checkPhoneQuest.OnQuestStart += DisplayPhoneTutorial;

        _mechanicContact.OnCheckNew += _checkPhoneQuest.EndQuest;
    }

    private void Update()
    {
        ManageTutorials();
    }

    private void ChangePhoneSetup()
    {
        GameManager.Instance.CurrentPhoneSetup = _phoneSetupWithMechanic;

        PhoneScreen phoneScreen = FindObjectOfType<PhoneScreen>();

        if (phoneScreen != null)
        {
            phoneScreen.DisplayContactsMenu();
        }
    }

    private void DisplayPhoneTutorial()
    {
        _phoneTutorial = Instantiate(_phoneTutorialPrefab);
    }

    private void ManageTutorials()
    {
        if (_phoneTutorial != null && PlayerKeyboardMap.GrabItem2.WasPerformedThisFrame())
        {
            Destroy(_phoneTutorial);
            _useItemTutorial = Instantiate(_useItemTutorialPrefab);
        }

        if (_useItemTutorial != null && PlayerMouseMap.UseItem.WasPerformedThisFrame())
        {
            Destroy(_useItemTutorial);
        }
    }
}
