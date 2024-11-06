using System;

public class HearingAid : Interactable
{
    public event Action OnInteract;

    public override void Interact()
    {
        OnInteract?.Invoke();
        Destroy(transform.parent.gameObject);
    }
}
