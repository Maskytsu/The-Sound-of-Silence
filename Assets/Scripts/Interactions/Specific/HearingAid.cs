using System;

public class HearingAid : Interactable
{
    protected override void Interact()
    {
        Destroy(gameObject);
    }
}
