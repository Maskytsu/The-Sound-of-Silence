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
 
    private void Start()
    {
        _nameTMP.text = Contact.Name;
        _pictureImage.texture = Contact.Picture;
    }

    public void DisplayMessagesMenu()
    {
        PhoneScreen.DisplayMessagesMenu(Contact);
    }
}
