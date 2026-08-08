using DG.Tweening;
using FMODUnity;
using NaughtyAttributes;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhoneScreen : MonoBehaviour
{
    public event Action OnFlashlightToggled;

    [ReadOnly] public ContactScriptable CurrentContact;

    [Header("Contacts Menu")]
    [SerializeField] private Transform _contactsLayout;
    [SerializeField] private ContactButton _contactButtonPrefab;
    [Header("Messages Menu")]
    [SerializeField] private Transform _messagesMenu;
    [SerializeField] private CanvasGroup _messagesMenuCanvasGroup;
    [SerializeField] private GameObject _glitchOverlay;
    [SerializeField] private Transform _messagesLayout;
    [SerializeField] private MessageTextBox _messageTextBoxPrefab;
    [Space]    
    [SerializeField] private TextMeshProUGUI _contactNameTMP;
    [SerializeField] private Button _callButton;
    [SerializeField] private Button _sendMessageButton;
    [SerializeField] private TextMeshProUGUI _sendMessageText;
    [SerializeField] private ContactScriptable _chrisContact;

    public GameObject FlashlightLightIcon;

    private PhoneMenuType _currentMenu;
    private Vector3 _messageMenuOutsideScreenPos;
    private Tween _messageMenuMoveTween;

    private void Awake()
    {
        _messageMenuOutsideScreenPos = _messagesMenu.localPosition;
        _messagesMenuCanvasGroup.interactable = false;
        FlashlightLightIcon.SetActive(false);
    }

    private void Start()
    {
        DisplayContactsMenu(false);
    }

    public void OnReturnButton()
    {
        if (_currentMenu == PhoneMenuType.MessagesMenu)
        {
            DisplayContactsMenu(true);
            return;
        }
    }

    public void OnFlashlightButton()
    {
        OnFlashlightToggled?.Invoke();
    }

    public void CallToCurrentContact()
    {
        RuntimeManager.PlayOneShot(FmodEvents.Instance.PhoneCallButton);
        CurrentContact.Call();
    }

    public void SendMessageToCurrentContact()
    {
        CurrentContact.SendMessage();
        RuntimeManager.PlayOneShot(FmodEvents.Instance.PhoneSendButton);
        DisplayMessagesMenu(CurrentContact);
    }

    public void DisplayContactsMenu(bool playBackSound = true)
    {
        _currentMenu = PhoneMenuType.ContactsMenu;
        if (playBackSound) RuntimeManager.PlayOneShot(FmodEvents.Instance.PhoneBackButton);

        foreach (Transform oldContact in _contactsLayout)
        {
            Destroy(oldContact.gameObject);
        }

        foreach (var contact in PhoneManager.Instance.CurrentPhoneSetup.Contacts)
        {
            ContactButton contactButton = Instantiate(_contactButtonPrefab, _contactsLayout);
            contactButton.PhoneScreen = this;
            contactButton.Contact = contact;
            contactButton.IsGlitched = IsContactGlitched(contact);
        }

        CurrentContact = null;
        _glitchOverlay.SetActive(false);

        //-------------
        if (_messageMenuMoveTween != null)
        {
            _messageMenuMoveTween.Kill();
            _messageMenuMoveTween.onComplete = null;
        }
        _messageMenuMoveTween?.Kill();
        _messageMenuMoveTween = _messagesMenu.DOLocalMove(_messageMenuOutsideScreenPos, 0.2f);
    }

    public void DisplayMessagesMenu(ContactScriptable contact)
    {
        _currentMenu = PhoneMenuType.MessagesMenu;

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
            _sendMessageText.text = "...";
        }
        else if (CurrentContact.IsMessageable && !CheckIfMessageWasSent())
        {
            _sendMessageButton.interactable = true;
            _sendMessageText.text = CurrentContact.MessageToSend.Text;
        }
        else
        {
            _sendMessageButton.interactable = false;
            _sendMessageText.text = "...";
        }

        if (CurrentContact.IsCallable) _callButton.interactable = true;
        else _callButton.interactable = false;

        _glitchOverlay.SetActive(IsContactGlitched(CurrentContact));

        //-------------
        if (_messageMenuMoveTween != null)
        {
            _messageMenuMoveTween.Kill();
            _messageMenuMoveTween.onComplete = null;
        }
        _messageMenuMoveTween = _messagesMenu.DOLocalMove(Vector3.zero, 0.2f);
        _messageMenuMoveTween.onComplete += () => { if (_messagesMenuCanvasGroup) _messagesMenuCanvasGroup.interactable = true; };
    }

    private bool IsContactGlitched(ContactScriptable contact) => contact == _chrisContact && !GameState.Instance.ReadDivorcePapers;

    private bool CheckIfMessageWasSent()
    {
        GameState.Instance.CheckContactState(CurrentContact, out bool? contactChecked, out bool? contactMessaged, out bool? contactCalled);

        if (contactMessaged != null)
        {
            return contactMessaged.Value;
        }
        else
        {
            Debug.LogError("Contact is messageable but it's state isn't handled. Returned as if it wasn't sent");
            return false;
        }
    }

    private enum PhoneMenuType
    {
        ContactsMenu = 0,
        MessagesMenu = 1,
    }
}
