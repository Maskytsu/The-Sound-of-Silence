using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContactButton : MonoBehaviour
{
    public ContactScriptable Contact;
    public PhoneScreen PhoneScreen;

    [SerializeField] private TextMeshProUGUI _nameTMP;
    [SerializeField] private RawImage _pictureImage;
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Color _newContactColor = Color.yellow;

    private void Start()
    {
        _nameTMP.text = Contact.Name;
        _pictureImage.texture = Contact.Picture;
        if(Contact.IsNew && !CheckIfContactWasChecked()) _backgroundImage.color = _newContactColor; ;
    }

    public void CheckContact()
    {
        if (Contact.IsNew && !CheckIfContactWasChecked()) Contact.CheckNew();
        RuntimeManager.PlayOneShot(FmodEvents.Instance.PhoneContactButton);
        PhoneScreen.DisplayMessagesMenu(Contact);
    }

    private bool CheckIfContactWasChecked()
    {
        GameState.Instance.CheckContactState(Contact, out bool? contactChecked, out bool? contactMessaged, out bool? contactCalled);

        if (contactChecked != null)
        {
            if (!contactChecked.Value)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            Debug.LogError("Contact is new but it's state isn't handled. Returned as if it was checked.");
            return true;
        }
    }
}
