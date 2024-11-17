using System;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public event Action OnInteract;

    [SerializeField] protected InteractionHitbox _interactionHitbox;
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
        _interactionHitbox.OnPointed += ShowPrompt;
        _interactionHitbox.OnUnpointed += HidePrompt;
        _interactionHitbox.OnInteract += Interact;
        _interactionHitbox.OnInteract += () => OnInteract?.Invoke();
    }
}
