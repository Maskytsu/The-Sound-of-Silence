using System;

public class Bed : Interactable
{
    public Action OnInteract;

    public override void Interact()
    {
        OnInteract?.Invoke();
    }
}
