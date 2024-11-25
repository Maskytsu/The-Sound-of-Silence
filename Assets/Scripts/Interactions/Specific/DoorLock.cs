using System.Collections;
using UnityEngine;

public class DoorLock : Unlockable
{
    public InteractionHitbox UnlockableHitbox => _unlockableHitbox;

    protected override void Unlock()
    {
        if (_locked)
        {
            _locked = false;
            HidePrompt();
            StartCoroutine(SwapHitboxesDelayed());
        }
    }

    private IEnumerator SwapHitboxesDelayed()
    {
        yield return new WaitForSeconds(0.25f);
        UpdateHitboxes();
    }
}