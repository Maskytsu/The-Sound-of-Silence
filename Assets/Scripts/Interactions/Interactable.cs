using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class Interactable : MonoBehaviour
{
    public event Action OnInteract;

    [SerializeField] protected InteractionHitbox _interactionHitbox;
    [SerializeField] protected Canvas _promptInteract;
    [SerializeField] private Outline _outline;
    [SerializeField] private Outline.Mode _outlineMode = Outline.Mode.OutlineAll;
    [Space]
    [SerializeField] private UnityEvent OnInteractUE = new();


    protected virtual bool ShowTutorial => false;
    protected virtual string GizmoIconName => "BlueInteractionIcon.png";
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
        if (ShowTutorial && _promptInteract != null) _promptInteract.enabled = true;
        _outline.enabled = true;
    }

    protected virtual void HidePromptAndOutline()
    {
        if (_promptInteract != null) _promptInteract.enabled = false;
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
        OnInteractUE?.Invoke();
        OnInteract?.Invoke();
    }

    public void SetupOutline(string htmlColor = "#e0da22")
    {
        ColorUtility.TryParseHtmlString(htmlColor, out var outlineColor);
        _outline.OutlineMode = _outlineMode;
        _outline.OutlineColor = outlineColor;
        _outline.OutlineWidth = 6.0f;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (SceneViewGizmoSettings.DrawInteractableGizmo && _interactionHitbox != null) 
        {
            if (!SceneViewGizmoSettings.DivideInteractableGizmo)
            {
                Gizmos.DrawIcon(_interactionHitbox.transform.position, "WhiteInteractionIcon.png", true);
                return;
            }

            if (_interactionHitbox.gameObject.activeSelf) Gizmos.DrawIcon(_interactionHitbox.transform.position, GizmoIconName, true);
            else Gizmos.DrawIcon(_interactionHitbox.transform.position, "TransInteractionIcon.png", true);
        }
    }
#endif
}