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
    [SerializeField] private Outline.Mode _outlineMode = Outline.Mode.OutlineAll;

    protected virtual bool ShowTutorial => false;
    public Outline Outline => _outline;

    [Button]
    protected abstract void Interact();

    protected virtual void Awake()
    {
        AssignMethodsToEvents();
        SetupOutline();
        HidePromptAndOutline();
    }

    protected virtual void OnDisable()
    {
        HidePromptAndOutline();
    }

    protected virtual void ShowPromptAndOutline()
    {
        if (ShowTutorial) _promptInteract.enabled = true;
        _outline.enabled = true;
    }

    protected virtual void HidePromptAndOutline()
    {
        _promptInteract.enabled = false;
        _outline.enabled = false;
    }

    private void AssignMethodsToEvents()
    {
        _interactionHitbox.OnPointed += ShowPromptAndOutline;
        _interactionHitbox.OnUnpointed += HidePromptAndOutline;
        _interactionHitbox.OnInteract += HandleInteraction;
    }

    private void HandleInteraction()
    {
        Interact();
        OnInteract?.Invoke();
    }

    public void SetupOutline(string htmlColor = "#e0da22")
    {
        ColorUtility.TryParseHtmlString(htmlColor, out var outlineColor);
        _outline.OutlineMode = _outlineMode;
        _outline.OutlineColor = outlineColor;
        _outline.OutlineWidth = 6.0f;
    }
}
