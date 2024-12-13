using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using RenderPipeline = UnityEngine.Rendering.RenderPipelineManager;

public class PortalCamera : MonoBehaviour
{
    [SerializeField] private Renderer[] _portals = new Renderer[2];

    [SerializeField] private Camera _playerCamera;
    [SerializeField] private Camera _portalCamera;

    private RenderTexture _portal1Texture;
    private RenderTexture _portal2Texture;

    private int _iterations = 7;

    private void Awake()
    {
        _portal1Texture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
        _portal2Texture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
    }

    private void Start()
    {
        _portals[0].material.mainTexture = _portal1Texture;
        _portals[1].material.mainTexture = _portal2Texture;
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
        if (_portals[0].isVisible)
        {
            _portalCamera.targetTexture = _portal1Texture;
            for (int i = _iterations - 1; i >= 0; --i)
            {
                RenderCamera(_portals[0], _portals[1], i, SRC);
            }
        }

        if(_portals[1].isVisible)
        {
            _portalCamera.targetTexture = _portal2Texture;
            for (int i = _iterations - 1; i >= 0; --i)
            {
                RenderCamera(_portals[1], _portals[0], i, SRC);
            }
        }
    }

    private void RenderCamera(Renderer inPortal, Renderer outPortal, int iterationID, ScriptableRenderContext SRC)
    {
        Transform inTransform = inPortal.transform;
        Transform outTransform = outPortal.transform;

        Transform portalCameraTransform = _portalCamera.transform;
        portalCameraTransform.position = _playerCamera.transform.position;
        portalCameraTransform.rotation = _playerCamera.transform.rotation;

        for(int i = 0; i <= iterationID; ++i)
        {
            // Position the camera behind the other portal.
            Vector3 relativePos = inTransform.InverseTransformPoint(portalCameraTransform.position);
            relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
            portalCameraTransform.position = outTransform.TransformPoint(relativePos);

            // Rotate the camera to look through the other portal.
            Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * portalCameraTransform.rotation;
            relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeRot;
            portalCameraTransform.rotation = outTransform.rotation * relativeRot;
        }

        // Set the camera's oblique view frustum.
        Plane p = new Plane(-outTransform.forward, outTransform.position);
        Vector4 clipPlaneWorldSpace = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        Vector4 clipPlaneCameraSpace =
            Matrix4x4.Transpose(Matrix4x4.Inverse(_portalCamera.worldToCameraMatrix)) * clipPlaneWorldSpace;

        var newMatrix = _playerCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        _portalCamera.projectionMatrix = newMatrix;

        // Render the camera to its render target.
        UniversalRenderPipeline.RenderSingleCamera(SRC, _portalCamera);
    }
}