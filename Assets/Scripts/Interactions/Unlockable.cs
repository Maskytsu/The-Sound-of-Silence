using NaughtyAttributes;
using UnityEngine;

public abstract class Unlockable : MonoBehaviour
{
    [SerializeField] protected InteractionHitbox _unlockableHitbox;
    [SerializeField] protected InteractionHitbox _interactableHitbox;
    [SerializeField] private Interactable _interactable;
    [Space]
    [SerializeField] protected Canvas _promptUnlock;

    protected bool _locked = true;
    protected virtual string GizmoIconName => "RedInteractionIcon.png";

    [Button]
    protected virtual void Unlock()
    {
        _interactable.SetupOutline();
    }

    private void Awake()
    {
        AssignMethodsToEvents();
    }

    private void Start()
    {
        _interactable.SetupOutline("#cf190c");
    }

    protected virtual void ShowPrompt()
    {
        _interactable.Outline.enabled = true;
        _promptUnlock.enabled = true;
    }

    protected virtual void HidePrompt()
    {
        _interactable.Outline.enabled = false;
        _promptUnlock.enabled = false;
    }

    protected void AssignMethodsToEvents()
    {
        _unlockableHitbox.OnPointed += ShowPrompt;
        _unlockableHitbox.OnUnpointed += HidePrompt;
        _unlockableHitbox.OnInteract += Unlock;
    }

    protected void UpdateHitboxes()
    {
        _interactableHitbox.gameObject.SetActive(!_locked);
        _unlockableHitbox.gameObject.SetActive(_locked);
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (SceneViewGizmoSettings.DrawInteractableGizmo && _unlockableHitbox != null)
        {
            if (!SceneViewGizmoSettings.DivideInteractableGizmo)
            {
                Gizmos.DrawIcon(_unlockableHitbox.transform.position, "WhiteInteractionIcon.png", true);
                return;
            }

            if (_unlockableHitbox.gameObject.activeSelf) Gizmos.DrawIcon(_unlockableHitbox.transform.position, GizmoIconName, true);
            else Gizmos.DrawIcon(_unlockableHitbox.transform.position, "TransInteractionIcon.png", true);
        }
    }
#endif
}
