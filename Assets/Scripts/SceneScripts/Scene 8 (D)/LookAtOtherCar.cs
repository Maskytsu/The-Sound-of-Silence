using Cinemachine;
using FMODUnity;
using System.Collections;
using UnityEngine;

public class LookAtOtherCar : MonoBehaviour
{
    [Header("Scene Objects")]
    [SerializeField] private Transform _carPointTransform;
    [SerializeField] private Trigger _lookAtCarTrigger;

    private CameraManager CameraManager => CameraManager.Instance;

    private void Start()
    {
        _lookAtCarTrigger.OnObjectTriggerEnter += () =>
        {
            _lookAtCarTrigger.gameObject.SetActive(false);
            StartCoroutine(LookAtCar());
        };
    }

    private IEnumerator LookAtCar()
    {
        yield return StartCoroutine(CameraManager.LookAtTargetAnimation(_carPointTransform.transform, 1f, 0f));
        yield return new WaitForSeconds(0.2f);
        RuntimeManager.PlayOneShot(FmodEvents.Instance.CarBraking);
    }
}
