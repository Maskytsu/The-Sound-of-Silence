using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FenceGate : Unlockable
{
    [SerializeField] private Transform _gateTransform;
    [SerializeField] private Transform _lockTransform;

    public override void Unlock()
    {
        if (_closed)
        {
            _closed = false;
            HidePrompt();
            StartCoroutine(OpenGate());
        }
    }

    private IEnumerator OpenGate()
    {
        _lockTransform.localPosition = new Vector3(-1.16f, _lockTransform.localPosition.y, _lockTransform.localPosition.z);
        yield return new WaitForSeconds(0.5f);
        Quaternion startingRotation = _gateTransform.localRotation;
        Quaternion targetRotation = Quaternion.Euler(0, 250, 0);
        float timeCount = 0f;

        while (_gateTransform.localRotation != targetRotation)
        {
            _gateTransform.localRotation = Quaternion.Slerp(startingRotation, targetRotation, timeCount);
            timeCount += Time.deltaTime;
            yield return new WaitForSeconds(0);
        }
    }
}