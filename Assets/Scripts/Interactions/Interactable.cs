using System;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public event Action OnInteract;

    public InteractionHitbox InteractionHitbox;

    [SerializeField] protected Canvas _promptInteract;

    protected abstract void Interact();

    protected virtual void Awake()
    {
        AssignMethodsToEvents();
    }

    protected virtual void ShowPrompt()
    {
        _promptInteract.enabled = true;
    }

    protected virtual void HidePrompt()
    {
        _promptInteract.enabled = false;
    }

    protected void AssignMethodsToEvents()
    {
        InteractionHitbox.OnPointed += ShowPrompt;
        InteractionHitbox.OnUnpointed += HidePrompt;
        InteractionHitbox.OnInteract += Interact;
        InteractionHitbox.OnInteract += () => OnInteract?.Invoke();
    }
}
