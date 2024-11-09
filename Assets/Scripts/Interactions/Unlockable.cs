using NaughtyAttributes;
using UnityEngine;

public abstract class Unlockable : MonoBehaviour
{
    public InteractionHitbox UnlockableHitbox;

    [SerializeField] protected InteractionHitbox _interactableHitbox;
    [Space]
    [SerializeField] protected Canvas _promptUnlock;

    protected bool _locked = true;

    protected abstract void Unlock();

    private void Awake()
    {
        AssignMethodsToEvents();
        UpdateHitboxes();
    }

    protected virtual void ShowPrompt()
    {
        _promptUnlock.enabled = true;
    }

    protected virtual void HidePrompt()
    {
        _promptUnlock.enabled = false;
    }

    protected void AssignMethodsToEvents()
    {
        UnlockableHitbox.OnPointed += ShowPrompt;
        UnlockableHitbox.OnUnpointed += HidePrompt;
        UnlockableHitbox.OnInteract += Unlock;
    }

    protected void UpdateHitboxes()
    {
        _interactableHitbox.gameObject.SetActive(!_locked);
        UnlockableHitbox.gameObject.SetActive(_locked);
    }
}
