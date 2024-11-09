using System;

public class HearingAid : Interactable
{
    protected override void Interact()
    {
        gameObject.SetActive(false);
    }
}
