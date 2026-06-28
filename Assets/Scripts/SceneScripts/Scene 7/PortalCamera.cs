using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;

public class PortalCamera : MonoBehaviour
{
    [ReadOnly] public bool IsPlayerClose = false;
    [ReadOnly] public bool DisplayPortal = false;

    //those portal transforms must face the opposite side of the player view
    //it's forward vector must be towards the not visable side
    [SerializeField] private Transform _visablePortalTransform;
    [SerializeField] private Renderer _visablePortalRenderer;
    [Space]
    [SerializeField] private Transform _otherPortalTransform;
    [Space]
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private Camera _portalCamera;
    [SerializeField] private ItemFlashlight _flashlightCopy;
    [SerializeField] private RenderTexture _portalDisplayTexture;

    private void Start()
    {
        _portalDisplayTexture.width = Screen.width;
        _portalDisplayTexture.height = Screen.height;
        _portalDisplayTexture.depth = 24;
        _portalDisplayTexture.format = RenderTextureFormat.ARGB32;
    }

    private void LateUpdate()
    {
        if (DisplayPortal) ManageSecondFlashlight();
    }

    private void OnEnable()
    {
        RenderPipelineManager.beginCameraRendering += UpdateCamera;
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= UpdateCamera;
    }

    private void UpdateCamera(ScriptableRenderContext SRC, Camera camera)
    {
        if (!DisplayPortal)
        {
            _portalCamera.enabled = false;
            return;
        }

        if (_visablePortalRenderer.isVisible || IsPlayerClose)
        {
            _portalCamera.enabled = true;
            RenderCamera();
        }
        else
        {
            _portalCamera.enabled = false;
        }
    }

    private void RenderCamera()
    {
        _portalCamera.transform.position = _playerCamera.transform.position;
        _portalCamera.transform.rotation = _playerCamera.transform.rotation;

        //position the camera behind the other portal
        Vector3 relativePos = _visablePortalTransform.InverseTransformPoint(_portalCamera.transform.position);
        relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
        _portalCamera.transform.position = _otherPortalTransform.TransformPoint(relativePos);

        //rotate the camera to look through the other portal
        Quaternion relativeRot = Quaternion.Inverse(_visablePortalTransform.rotation) * _portalCamera.transform.rotation;
        relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeRot;
        _portalCamera.transform.rotation = _otherPortalTransform.rotation * relativeRot;

        //set the camera's oblique view frustum
        Plane plane = new Plane(-_otherPortalTransform.forward, _otherPortalTransform.position);
        Vector4 clipPlaneWorldSpace = new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance);
        Vector4 clipPlaneCameraSpace = Matrix4x4.Transpose(Matrix4x4.Inverse(_portalCamera.worldToCameraMatrix)) * clipPlaneWorldSpace;

        var newMatrix = _playerCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        _portalCamera.projectionMatrix = newMatrix;
    }

    private void ManageSecondFlashlight()
    {
        Item spawnedItem = PlayerObjects.Instance.PlayerEquipment.SpawnedItemInHand;

        if (spawnedItem is ItemFlashlight)
        {
            ItemFlashlight playerFlashlight = spawnedItem.GetComponent<ItemFlashlight>();
            _flashlightCopy.LightCone.SetActive(playerFlashlight.LightCone.activeSelf);
        }
        else
        {
            _flashlightCopy.LightCone.SetActive(false);
        }
    }
}