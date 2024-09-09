using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FenceGate))]
public class FenceGateLock : Unlockable
{
    [SerializeField] private Transform _lockTransform;

    public override void Unlock()
    {
        if (_locked)
        {
            _locked = false;
            HidePrompt();

            _lockTransform.localPosition = new Vector3(2.325f, _lockTransform.localPosition.y, _lockTransform.localPosition.z);

            StartCoroutine(ChangeLayerDelayed());
        }
    }

    private IEnumerator ChangeLayerDelayed()
    {
        yield return new WaitForSeconds(0.25f);
        gameObject.layer = _interactableLayer;
    }
}