using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPhoneHandler : MonoBehaviour
{
    [SerializeField] private QuestScriptable _checkPhoneQuest;
    [SerializeField] private ContactScriptable _mechanicContact;

    [SerializeField] private PhoneSetupScriptable _phoneSetupWithMechanic;

    private void Start()
    {
        _checkPhoneQuest.OnQuestStart += ChangePhoneSetup;
        _mechanicContact.OnCheckNew += _checkPhoneQuest.EndQuest;
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
}
