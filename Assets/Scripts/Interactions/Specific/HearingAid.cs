using FMODUnity;
using System;

public class HearingAid : Interactable
{
    protected override void Interact()
    {
        AudioManager.Instance.ChangeIsAbleToHear(true);
        RuntimeManager.PlayOneShot(FmodEvents.Instance.PuttingOnHearingAid);
        gameObject.SetActive(false);
    }
}
