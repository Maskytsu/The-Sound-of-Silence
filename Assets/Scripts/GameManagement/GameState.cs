using NaughtyAttributes;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }

    [ReadOnly] public bool MechanicChecked = false;
    [ReadOnly] public bool MechanicMessaged = false;
    [Space]
    [ReadOnly] public bool ClaireMessaged = false;
    [ReadOnly] public bool ClaireCalled = false;
    [Space]
    [ReadOnly] public bool PoliceChecked = false;
    [ReadOnly] public bool PoliceCalled = false;
    [Space]
    [ReadOnly] public bool TookPills = false;

    [Space]
    [SerializeField] private ContactScriptable _mechanicContact;
    [SerializeField] private ContactScriptable _claireInteractableContact;
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
        else if(contact == _claireInteractableContact)
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

        _claireInteractableContact.OnSendMessage += () => ClaireMessaged = true;
        _claireInteractableContact.OnCall += () => ClaireCalled = true;

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
