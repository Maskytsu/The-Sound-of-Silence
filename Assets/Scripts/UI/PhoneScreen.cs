using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhoneScreen : MonoBehaviour
{
    [SerializeField] private GameObject _contactsMenu;
    [SerializeField] private Transform _contactsLayout;
    [Space]
    [SerializeField] private GameObject _messagesMenu;
    [SerializeField] private Transform _messagesLayout;
    [Space]
    [SerializeField] private Transform _contactButtonPrefab;

    private List<PhoneSetupScriptable.PhoneContact> _contacts;

    private void Start()
    {
        _contacts = GameManager.Instance.CurrentPhoneSetup.Contacts;
        FillInContatcsMenu();
        ShowContactsMenu();
    }

    public void FillInContatcsMenu()
    {
        foreach(var contact in _contacts)
        {
            Transform buttonTransform = Instantiate(_contactButtonPrefab, _contactsLayout);
            TextMeshProUGUI buttonTMP = buttonTransform.Find("Name").GetComponent<TextMeshProUGUI>();
            RawImage buttonRawImage = buttonTransform.Find("Icon").Find("Picture").GetComponent<RawImage>();

            buttonTMP.text = contact.Name;
            buttonRawImage.texture = contact.Picture;
        }
    }

    public void ShowContactsMenu()
    {
        _contactsMenu.SetActive(true);
        _messagesMenu.SetActive(false);
    }

    public void DisplayMessagesMenu(PhoneSetupScriptable.PhoneContact contact)
    {
        _messagesMenu.SetActive(true);
        _contactsMenu.SetActive(false);
    }
}
