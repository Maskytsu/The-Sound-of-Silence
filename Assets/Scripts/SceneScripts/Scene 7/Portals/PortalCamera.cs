using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCamera : MonoBehaviour
{
    [SerializeField] private Camera _portalCamera;
    [SerializeField] private Material _portalDisplayMaterial;

    [SerializeField] private Transform _watchingPortal;
    [SerializeField] private Transform _otherPortal;

    private Transform PlayerCamera => CameraManager.Instance.CameraBrain.transform;

    private void Start()
    {
        CreateAndAssignTextureToDisplay();
    }

    private void LateUpdate()
    {
        MimicPlayerCamerasView();
    }

    private void MimicPlayerCamerasView()
    {
        Vector3 playerOffsetFromPortal = PlayerCamera.position - _otherPortal.position;
        _portalCamera.transform.position = _watchingPortal.position + playerOffsetFromPortal;

        float angularDifferenceBetweenPortalRotations = Quaternion.Angle(_watchingPortal.rotation, _otherPortal.rotation);

        Quaternion portalRotationalDifference = Quaternion.AngleAxis(angularDifferenceBetweenPortalRotations, Vector3.up);
        Vector3 newCameraDirection = portalRotationalDifference * PlayerCamera.forward;
        _portalCamera.transform.rotation = Quaternion.LookRotation(newCameraDirection, Vector3.up);
    }

    private void CreateAndAssignTextureToDisplay()
    {
        if (_portalCamera.targetTexture != null)
        {
            _portalCamera.targetTexture.Release();
        }

        _portalCamera.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        _portalDisplayMaterial.mainTexture = _portalCamera.targetTexture;
    }
}