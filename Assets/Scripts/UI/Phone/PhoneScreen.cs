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
    [Space]
    [Header("Messages Menu")]
    [SerializeField] private GameObject _messagesMenu;
    [SerializeField] private Transform _messagesLayout;
    [SerializeField] private Transform _messageTextBoxPrefab;
    [SerializeField] private Color _playersMessageBackgroundColor;
    [SerializeField] private TextMeshProUGUI _contactNameTMP;
    [SerializeField] private Button _callButton;
    [SerializeField] private Button _sendMessageButton;

    private List<ContactScriptable> _contacts;

    private void Start()
    {
        _contacts = GameManager.Instance.CurrentPhoneSetup.Contacts;
        FillInContatcsMenu();
        ShowContactsMenu();
    }

    public void CallToCurrentContact()
    {
        CurrentContact.Call();
    }

    public void SendMessageToCurrentContact()
    {
        CurrentContact.SendMessage();
    }

    public void ShowContactsMenu()
    {
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
        _contactNameTMP.text = contact.Name;

        foreach (var message in contact.Messages)
        {
            Transform messageTextBox = Instantiate(_messageTextBoxPrefab, _messagesLayout);
            RawImage messageBackground = messageTextBox.Find("TextBackground").GetComponent<RawImage>();
            TextMeshProUGUI messageTMP = messageBackground.transform.Find("Text").GetComponent<TextMeshProUGUI>();

            messageTMP.text = message.Text;
            if (message.IsPlayers) messageBackground.color = _playersMessageBackgroundColor;
        }

        if (contact.isCallable) _callButton.interactable = true;
        else _callButton.interactable = false;

        if (contact.isMessageable) _sendMessageButton.interactable = true;
        else _sendMessageButton.interactable = false;

        _messagesMenu.SetActive(true);
        _contactsMenu.SetActive(false);
    }

    private void FillInContatcsMenu()
    {
        foreach (var contact in _contacts)
        {
            ContactButton contactButton = Instantiate(_contactButtonPrefab, _contactsLayout);
            contactButton.PhoneScreen = this;
            contactButton.Contact = contact;
        }
    }
}