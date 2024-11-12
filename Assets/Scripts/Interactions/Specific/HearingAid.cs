using System;

public class HearingAid : Interactable
{
    protected override void Interact()
    {
        AudioManager.Instance.ChangeIsAbleToHear(true);
        gameObject.SetActive(false);
    }
}
