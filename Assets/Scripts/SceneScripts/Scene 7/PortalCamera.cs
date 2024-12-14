using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using RenderPipeline = UnityEngine.Rendering.RenderPipelineManager;

public class PortalCamera : MonoBehaviour
{
    //those portal transforms must face the opposite side of the player view
    //it's forward vector must be towards the not visable side
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

    private void Update()
    {
        //MOŻNA SPAWNOWAĆ LATARKĘ NA PORTAL KAMERZE ŻEBY BYŁO TAKIE SAMO ŚWIATŁO ZA I PRZED PORTALEM
        //POTWÓR MOŻE TEŻ TAM DAWAĆ JAKIEŚ ŚWIATŁO I TO MOŻE COŚ BUGOWAĆ - NA TRIGGERZE TEPAĆ POTWORA I WYŁĄCZAĆ MU ŚWIATŁO????
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

        //position the camera behind the other portal
        Vector3 relativePos = visablePortal.InverseTransformPoint(_portalCamera.transform.position);
        relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
        _portalCamera.transform.position = otherPortal.TransformPoint(relativePos);

        //rotate the camera to look through the other portal
        Quaternion relativeRot = Quaternion.Inverse(visablePortal.rotation) * _portalCamera.transform.rotation;
        relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeRot;
        _portalCamera.transform.rotation = otherPortal.rotation * relativeRot;

        //set the camera's oblique view frustum
        Plane plane = new Plane(-otherPortal.forward, otherPortal.position);
        Vector4 clipPlaneWorldSpace = new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance);
        Vector4 clipPlaneCameraSpace =
            Matrix4x4.Transpose(Matrix4x4.Inverse(_portalCamera.worldToCameraMatrix)) * clipPlaneWorldSpace;

        var newMatrix = _playerCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        _portalCamera.projectionMatrix = newMatrix;

        //render the camera to its render target
#pragma warning disable 618
        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! unity sends warning like this: 
        //RenderSingleCamera is obsolete, please use RenderPipeline.SubmitRenderRequest with UniversalRenderer.SingleCameraRequest as RequestData type
        UniversalRenderPipeline.RenderSingleCamera(SRC, _portalCamera);
        //this paragma things disables this warnings
#pragma warning restore 618
    }
}