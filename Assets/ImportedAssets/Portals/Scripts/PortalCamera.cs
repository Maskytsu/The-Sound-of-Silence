using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using RenderPipeline = UnityEngine.Rendering.RenderPipelineManager;

public class PortalCamera : MonoBehaviour
{
    [SerializeField] private Transform _visablePortalTransform;
    [SerializeField] private Renderer _visablePortalRenderer;
    [Space]
    [SerializeField] private Transform _otherPortalTransform;
    [Space]
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private Camera _portalCamera;
    [Space]
    [SerializeField] private Trigger _playerCloseTrigger;

    private RenderTexture _portalDisplayTexture;

    private bool _isPlayerClose = false;

    private void Awake()
    {
        _portalDisplayTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
    }

    private void Start()
    {
        _visablePortalRenderer.material.mainTexture = _portalDisplayTexture;

        _playerCloseTrigger.OnObjectTriggerEnter += () => _isPlayerClose = true;
        _playerCloseTrigger.OnObjectTriggerExit += () => _isPlayerClose = false;
    }

    private void OnEnable()
    {
        RenderPipeline.beginCameraRendering += UpdateCamera;
    }

    private void OnDisable()
    {
        RenderPipeline.beginCameraRendering -= UpdateCamera;
    }

    void UpdateCamera(ScriptableRenderContext SRC, Camera camera)
    {
        if (_visablePortalRenderer.isVisible || _isPlayerClose)
        {
            _portalCamera.targetTexture = _portalDisplayTexture;

            RenderCamera(_visablePortalTransform, _otherPortalTransform, SRC);
        }
    }

    private void RenderCamera(Transform visablePortal, Transform otherPortal, ScriptableRenderContext SRC)
    {
        _portalCamera.transform.position = _playerCamera.transform.position;
        _portalCamera.transform.rotation = _playerCamera.transform.rotation;

        // Position the camera behind the other portal.
        Vector3 relativePos = visablePortal.InverseTransformPoint(_portalCamera.transform.position);
        relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
        _portalCamera.transform.position = otherPortal.TransformPoint(relativePos);

        // Rotate the camera to look through the other portal.
        Quaternion relativeRot = Quaternion.Inverse(visablePortal.rotation) * _portalCamera.transform.rotation;
        relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeRot;
        _portalCamera.transform.rotation = otherPortal.rotation * relativeRot;

        // Set the camera's oblique view frustum.
        Plane p = new Plane(-otherPortal.forward, otherPortal.position);
        Vector4 clipPlaneWorldSpace = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        Vector4 clipPlaneCameraSpace =
            Matrix4x4.Transpose(Matrix4x4.Inverse(_portalCamera.worldToCameraMatrix)) * clipPlaneWorldSpace;

        var newMatrix = _playerCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        _portalCamera.projectionMatrix = newMatrix;

        // Render the camera to its render target.
        UniversalRenderPipeline.RenderSingleCamera(SRC, _portalCamera);
    }
}