using UnityEditor;
using UnityEngine;

public class GizmoSpheare : MonoBehaviour
{
    [SerializeField] private float _sphereRadius = 0.25f;
    [SerializeField] private Color _sphereColor = Color.white;
    [SerializeField] private bool _onlyWhenSelected = false;

    private void OnDrawGizmos()
    {
        if (!_onlyWhenSelected) DrawSphere();
    }

    private void OnDrawGizmosSelected()
    {
        if (_onlyWhenSelected) DrawSphere();
    }

    private void DrawSphere()
    {
        Gizmos.color = _sphereColor;
        Gizmos.DrawSphere(gameObject.transform.position, _sphereRadius);
    }
}
