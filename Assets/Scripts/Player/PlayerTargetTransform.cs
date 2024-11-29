using NaughtyAttributes;
using UnityEngine;

[ExecuteInEditMode]
//v IF ENTERING PLAYMODE WITH DOMAIN RELOAD DISABLED
//There are no additional OnDisable or OnEnable calls for scripts marked with the [ExecuteInEditMode] or [ExecuteAlways].
public class PlayerTargetTransform : MonoBehaviour
{
    [ReadOnly] public Vector3 Position;
    [ReadOnly] public Vector3 Rotation;
    [Space]
    [SerializeField] private bool _applyGravity = false;
    [SerializeField] private bool _showGizmos = true;
    [SerializeField] private Transform _camera;
    [SerializeField] private CharacterController _characterController;

    private void Awake()
    {
        UpdatePositionAndRotation();
    }

    private void Update()
    {
        UpdatePositionAndRotation();
        CreateSimpleGravity();
    }

    private void OnValidate()
    {
        UpdatePositionAndRotation();
    }

    private void OnDrawGizmos()
    {
        if (_showGizmos)
        {
            DrawInteractor();
            DrawCharacterController();
        }
    }

    private void UpdatePositionAndRotation()
    {
        Position = transform.position;
        Rotation = new Vector3(_camera.eulerAngles.x, transform.eulerAngles.y, 0);
    }

    private void CreateSimpleGravity()
    {
        if (_applyGravity)
        {
            _characterController.Move(Vector3.down * 3f);
        }
    }

    private void DrawInteractor()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(_camera.position, _camera.forward * 2f);
    }

    private void DrawCharacterController()
    {
        Gizmos.color = Color.cyan;
        Gizmos.matrix = transform.localToWorldMatrix;

        Vector3 center = _characterController.center;
        float height = _characterController.height;
        float radius = _characterController.radius;

        Vector3 topSphereCenter = center;
        Vector3 bottomSphereCenter = center;

        topSphereCenter.y += (height / 2) - radius;
        bottomSphereCenter.y -= (height / 2) - radius;

        Gizmos.DrawWireSphere(topSphereCenter, radius);
        Gizmos.DrawWireSphere(bottomSphereCenter, radius);

        Gizmos.DrawLine(topSphereCenter + Vector3.forward * radius, bottomSphereCenter + Vector3.forward * radius);
        Gizmos.DrawLine(topSphereCenter - Vector3.forward * radius, bottomSphereCenter - Vector3.forward * radius);
        Gizmos.DrawLine(topSphereCenter + Vector3.right * radius, bottomSphereCenter + Vector3.right * radius);
        Gizmos.DrawLine(topSphereCenter - Vector3.right * radius, bottomSphereCenter - Vector3.right * radius);
    }
}
