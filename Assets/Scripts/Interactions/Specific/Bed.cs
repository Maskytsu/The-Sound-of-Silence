using System;

public class Bed : Interactable
{
    public event Action OnInteract;

    public override void Interact()
    {
        OnInteract?.Invoke();
    }
}
