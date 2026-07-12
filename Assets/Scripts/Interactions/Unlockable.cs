using DG.Tweening;
using NaughtyAttributes;
using System.Drawing;
using UnityEngine;
using UnityEngine.Events;

public abstract class Unlockable : MonoBehaviour
{
    [SerializeField] protected InteractionHitbox _unlockableHitbox;
    [SerializeField] protected InteractionHitbox _interactableHitbox;
    [SerializeField] private Interactable _interactable;
    [Space]
    [SerializeField] protected Canvas _promptUnlock;
    [Space]
    [SerializeField] private UnityEvent OnUnlockUE = new();

    protected bool _locked = true;
    protected virtual string GizmoIconName => "RedInteractionIcon.png";
    private Tween _promptFadeTween;
    private CanvasGroup _promptUnlockGroup;


    [Button]
    protected virtual void Unlock()
    {
        OnUnlockUE?.Invoke();
        _interactable.SetupOutline();
    }

    private void Awake()
    {
        AssignMethodsToEvents();
        _promptUnlock.enabled = true;
        _promptUnlockGroup = _promptUnlock.GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        _interactable.SetupOutline(UIColors.Instance.UnlockableOutline);
    }

    protected void ShowPrompt()
    {
        _interactable.Outline.enabled = true;

        _promptFadeTween?.Kill();
        _promptFadeTween = _promptUnlockGroup.DOFade(1.0f, 0.2f);
    }

    protected void HidePrompt()
    {
        _interactable.Outline.enabled = false;

        _promptFadeTween?.Kill();
        _promptFadeTween = _promptUnlockGroup.DOFade(0.0f, 0.2f);
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
