using System;
using UnityEngine;

public class InteractionHitbox : MonoBehaviour
{
    public Action OnInteract;
    public Action OnPointed;
    public Action OnUnpointed;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!SceneViewGizmoSettings.DrawInteractionHitbox) return;

        BoxCollider boxTrigger = GetComponent<BoxCollider>();

        Gizmos.color = Color.whiteSmoke;
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);

        if (SceneViewGizmoSettings.DrawFullTriggers) Gizmos.DrawCube(boxTrigger.center, boxTrigger.size);
        else Gizmos.DrawCube(boxTrigger.center, boxTrigger.size);
        Gizmos.matrix = oldMatrix;
    }
#endif
}