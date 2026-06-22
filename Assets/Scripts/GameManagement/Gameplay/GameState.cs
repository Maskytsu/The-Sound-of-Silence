using NaughtyAttributes;
using UnityEngine;

public class GameState : SingletonMonobehaviour<GameState>
{
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
    public bool TookKeys = false;
    [Space]
    public bool ReadConcertTicket = false;
    public bool ReadDivorcePapers = false;
    public bool ReadNewspaper = false;

    [Space]
    [SerializeField] private ContactScriptable _mechanicContact;
    [SerializeField] private ContactScriptable _claireInteractableContact;
    [SerializeField] private ContactScriptable _policeContact;

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

    [Button]
    private void SetupGoodEnding() 
    {
        ClaireMessaged = true;
        TookPills = true;
        ReadNewspaper = true;
    }

    [Button]
    private void SetupBadEnding()
    {
        ClaireMessaged = false;
        ClaireCalled = false;
        PoliceCalled = false;
        TookPills = false;
        ReadNewspaper = false;
    }
}
