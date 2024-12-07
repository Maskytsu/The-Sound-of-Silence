using NaughtyAttributes;
using System;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public Action OnObjectTriggerEnter;

    [Layer] public int Layer;
    public Color GizmoColor;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == Layer)
        {
            OnObjectTriggerEnter?.Invoke();
        }
    }

    private void OnDrawGizmos()
    {
        BoxCollider boxTrigger = GetComponent<BoxCollider>();

        Gizmos.color = GizmoColor;
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);

        Gizmos.DrawWireCube(boxTrigger.center, boxTrigger.size);
        Gizmos.matrix = oldMatrix;
    }
}
