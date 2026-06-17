using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class Interactable : MonoBehaviour
{
    public event Action OnInteract;

    [SerializeField] protected InteractionHitbox _interactionHitbox;
    [SerializeField] protected Canvas _promptInteract;
    [SerializeField] private Outline _outline;

    protected virtual bool ShowTutorial => false;
    public Outline Outline => _outline;

    [Button]
    protected abstract void Interact();

    protected virtual void Awake()
    {
        AssignMethodsToEvents();
        SetupOutline();
        _promptInteract.enabled = false;
    }

    protected virtual void ShowPrompt()
    {
        if (ShowTutorial) _promptInteract.enabled = true;
        _outline.enabled = true;
    }

    protected virtual void HidePrompt()
    {
        _promptInteract.enabled = false;
        _outline.enabled = false;
    }

    private void AssignMethodsToEvents()
    {
        _interactionHitbox.OnPointed += ShowPrompt;
        _interactionHitbox.OnUnpointed += HidePrompt;
        _interactionHitbox.OnInteract += Interact;
        _interactionHitbox.OnInteract += () => OnInteract?.Invoke();
    }

    public void SetupOutline(string htmlColor = "#e0da22")
    {
        ColorUtility.TryParseHtmlString(htmlColor, out var outlineColor);
        _outline.enabled = false;
        _outline.OutlineMode = Outline.Mode.OutlineAll;
        _outline.OutlineColor = outlineColor;
        _outline.OutlineWidth = 6.0f;
    }
}
