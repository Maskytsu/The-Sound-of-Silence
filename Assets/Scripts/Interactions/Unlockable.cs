using NaughtyAttributes;
using UnityEngine;

public abstract class Unlockable : MonoBehaviour
{
    [SerializeField] protected InteractionHitbox _unlockHitbox;
    [SerializeField] protected Canvas _promptUnlock;
    [Space]
    [SerializeField] protected GameObject _unlockableHitbox;
    [SerializeField] protected GameObject _interactableHitbox;

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
        _unlockHitbox.OnPointed += ShowPrompt;
        _unlockHitbox.OnUnpointed += HidePrompt;
        _unlockHitbox.OnInteract += Unlock;
    }

    protected void UpdateHitboxes()
    {
        _interactableHitbox.SetActive(!_locked);
        _unlockableHitbox.SetActive(_locked);
    }
}
