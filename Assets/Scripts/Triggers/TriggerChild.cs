using UnityEngine;

public class TriggerChild : MonoBehaviour
{
    [SerializeField] private Trigger _triggerParent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == _triggerParent.Layer)
        {
            _triggerParent.OnObjectTriggerEnter?.Invoke();
            gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        BoxCollider boxTrigger = GetComponent<BoxCollider>();

        Gizmos.color = _triggerParent.GizmoColor;
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);

        if (_triggerParent.DrawWireCube) Gizmos.DrawWireCube(boxTrigger.center, boxTrigger.size);
        else Gizmos.DrawCube(boxTrigger.center, boxTrigger.size);
        Gizmos.matrix = oldMatrix;
    }
}
