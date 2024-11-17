using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhoneScreen : MonoBehaviour
{
    public ContactScriptable CurrentContact;

    [Header("Contacts Menu")]
    [SerializeField] private GameObject _contactsMenu;
    [SerializeField] private Transform _contactsLayout;
    [SerializeField] private ContactButton _contactButtonPrefab;
    [Header("Messages Menu")]
    [SerializeField] private GameObject _messagesMenu;
    [SerializeField] private Transform _messagesLayout;
    [SerializeField] private MessageTextBox _messageTextBoxPrefab;

    [SerializeField] private TextMeshProUGUI _contactNameTMP;
    [SerializeField] private Button _callButton;
    [SerializeField] private Button _sendMessageButton;

    private void Start()
    {
        DisplayContactsMenu();
    }

    public void CallToCurrentContact()
    {
        CurrentContact.Call();
        PlayerObjectsHolder.Instance.PlayerEquipment.ChangeItem(ItemType.NONE);
    }

    public void SendMessageToCurrentContact()
    {
        CurrentContact.SendMessage();
        DisplayMessagesMenu(CurrentContact);
    }

    public void DisplayContactsMenu()
    {
        foreach (Transform oldContact in _contactsLayout)
        {
            Destroy(oldContact.gameObject);
        }

        foreach (var contact in GameManager.Instance.CurrentPhoneSetup.Contacts)
        {
            ContactButton contactButton = Instantiate(_contactButtonPrefab, _contactsLayout);
            contactButton.PhoneScreen = this;
            contactButton.Contact = contact;
        }

        CurrentContact = null;

        _contactsMenu.SetActive(true);
        _messagesMenu.SetActive(false);
    }

    public void DisplayMessagesMenu(ContactScriptable contact)
    {
        foreach (Transform oldMessage in _messagesLayout)
        {
            Destroy(oldMessage.gameObject);
        }

        CurrentContact = contact;
        _contactNameTMP.text = CurrentContact.Name;

        foreach (var message in CurrentContact.Messages)
        {
            MessageTextBox messageTextBox = Instantiate(_messageTextBoxPrefab, _messagesLayout);
            messageTextBox.Message = message;
        }

        if (CurrentContact.IsMessageable && CheckIfMessageWasSent())
        {
            MessageTextBox messageTextBox = Instantiate(_messageTextBoxPrefab, _messagesLayout);
            messageTextBox.Message = CurrentContact.MessageToSend;
            _sendMessageButton.interactable = false;
        }
        else if (CurrentContact.IsMessageable && !CheckIfMessageWasSent())
        {
            _sendMessageButton.interactable = true;
        }
        else
        {
            _sendMessageButton.interactable = false;
        }

        if (CurrentContact.IsCallable && AudioManager.Instance.IsAbleToHear) _callButton.interactable = true;
        else _callButton.interactable = false;

        _messagesMenu.SetActive(true);
        _contactsMenu.SetActive(false);
    }

    private bool CheckIfMessageWasSent()
    {
        GameState.Instance.CheckContactState(CurrentContact, out bool? contactChecked, out bool? contactMessaged, out bool? contactCalled);

        if (contactMessaged != null)
        {
            if (contactMessaged.Value)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            Debug.LogError("Contact is messageable but it's state isn't handled. Returned as if it wasn't sent");
            return false;
        }
    }
}
