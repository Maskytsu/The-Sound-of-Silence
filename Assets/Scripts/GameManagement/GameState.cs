using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }

    public bool MechanicChecked = false;
    public bool MechanicMessaged = false;
    [Space]
    public bool ClaireMessaged = false;
    public bool ClaireCalled = false;
    [Space]
    public bool PoliceChecked = false;
    public bool PoliceCalled = false;
    [Space]
    public bool TookPills = false;
    [Space]
    [SerializeField] private ContactScriptable _mechanicContact;
    [SerializeField] private ContactScriptable _claireContact;
    [SerializeField] private ContactScriptable _policeContact;

    private void Awake()
    {
        CreateInstance();

        ListenToPhoneEvents();
    }

    public void CheckContactState(ContactScriptable contact, out bool? contactChecked, out bool? contactMessaged, out bool? contactCalled)
    {
        contactChecked = null;
        contactMessaged = null;
        contactCalled = null;

        if(contact == _mechanicContact)
        {
            contactChecked = MechanicChecked;
            contactMessaged = MechanicMessaged;
        }
        else if(contact == _claireContact)
        {
            contactMessaged = ClaireMessaged;
            contactCalled = ClaireCalled;
        }
        else if(contact == _policeContact)
        {
            contactChecked = PoliceChecked;
            contactMessaged = PoliceCalled;
        }
    }

    private void ListenToPhoneEvents()
    {
        _mechanicContact.OnCheckNew += () => MechanicChecked = true;
        _mechanicContact.OnSendMessage += () => MechanicMessaged = true;

        _claireContact.OnSendMessage += () => ClaireMessaged = true;
        _claireContact.OnCall += () => ClaireCalled = true;

        _policeContact.OnCheckNew += () => PoliceChecked = true;
        _policeContact.OnCall += () => PoliceCalled = true;
    }

    private void CreateInstance()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one GameState in the scene.");
        }
        Instance = this;
    }
}
