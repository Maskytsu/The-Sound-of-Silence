using UnityEngine;
using UnityEngine.Rendering;
using RenderPipeline = UnityEngine.Rendering.RenderPipelineManager;

public class Portal : MonoBehaviour
{
    [SerializeField] private Camera _portalCamera;
    [SerializeField] private Material _portalDisplayMaterial;
    [SerializeField] private Transform _thisPortal;
    [SerializeField] private Transform _otherPortal;

    private Transform PlayerCamera => CameraManager.Instance.CameraBrain.transform;

    private void Start()
    {
        CreateAndAssignTextureToDisplay();
    }

    //private void OnPreCull()
    private void OnEnable()
    {
        RenderPipeline.beginCameraRendering += UpdateCameraToMimicPlayerCamerasView;
    }

    private void OnDisable()
    {
        RenderPipeline.beginCameraRendering -= UpdateCameraToMimicPlayerCamerasView;
    }

    private void UpdateCameraToMimicPlayerCamerasView(ScriptableRenderContext SRC, Camera camera)
    {
        Vector3 playerOffsetFromPortal = PlayerCamera.position - _otherPortal.position;
        _portalCamera.transform.position = _thisPortal.position + playerOffsetFromPortal;

        float angularDifferenceBetweenPortalRotations = Quaternion.Angle(_thisPortal.rotation, _otherPortal.rotation);

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
