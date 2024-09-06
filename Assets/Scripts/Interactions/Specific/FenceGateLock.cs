using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FenceGate))]
public class FenceGateLock : Unlockable
{
    [SerializeField] private Transform _lockTransform;

    private void Awake()
    {
        GetComponent<FenceGate>().enabled = false;
    }

    public override void Unlock()
    {
        if (_locked)
        {
            HidePrompt();
            _locked = false;

            GetComponent<FenceGate>().enabled = true;
            StartCoroutine(ChangeLayer());

            _lockTransform.localPosition = new Vector3(2.325f, _lockTransform.localPosition.y, _lockTransform.localPosition.z);
        }
    }

    private IEnumerator ChangeLayer()
    {
        yield return new WaitForSeconds(0.25f);
        gameObject.layer = (int)Mathf.Log(_interactableMask.value, 2);
    }
}