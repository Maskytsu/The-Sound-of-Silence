using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PhoneSetup", menuName = "ScriptableObjects/PhoneSetup")]
public class PhoneSetupScriptable : ScriptableObject
{
    public List<ContactScriptable> Contacts;

    public void ClearSubscribersFromContacts()
    {
        foreach(var contact in Contacts)
        {
            contact.ClearSubscribers();
        }
    }
}
