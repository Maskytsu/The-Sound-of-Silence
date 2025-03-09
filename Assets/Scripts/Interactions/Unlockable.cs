using NaughtyAttributes;
using UnityEngine;

public abstract class Unlockable : MonoBehaviour
{
    [SerializeField] protected InteractionHitbox _unlockableHitbox;
    [SerializeField] protected InteractionHitbox _interactableHitbox;
    [Space]
    [SerializeField] protected Canvas _promptUnlock;

    protected bool _locked = true;

    [Button]
    protected abstract void Unlock();

    private void Awake()
    {
        AssignMethodsToEvents();
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
        _unlockableHitbox.OnPointed += ShowPrompt;
        _unlockableHitbox.OnUnpointed += HidePrompt;
        _unlockableHitbox.OnInteract += Unlock;
    }

    protected void UpdateHitboxes()
    {
        _interactableHitbox.gameObject.SetActive(!_locked);
        _unlockableHitbox.gameObject.SetActive(_locked);
    }
}
