using System.Collections;
using UnityEngine;

public class FenceGateLock : Unlockable
{
    public InteractionHitbox UnlockableHitbox => _unlockableHitbox;
    public InteractionHitbox InteractableHitbox => _interactableHitbox;

    [Space]
    [SerializeField] private Transform _lockTransform;

    protected override void Unlock()
    {
        if (_locked)
        {
            _locked = false;
            HidePrompt();

            _lockTransform.localPosition = new Vector3(2.325f, _lockTransform.localPosition.y, _lockTransform.localPosition.z);

            StartCoroutine(SwapHitboxesDelayed());
        }
    }

    private IEnumerator SwapHitboxesDelayed()
    {
        yield return new WaitForSeconds(0.25f);
        UpdateHitboxes();
    }
}