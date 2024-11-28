using NaughtyAttributes;
using System;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    public Action OnPlayerTriggerEnter;
    public Action OnPlayerTriggerExit;

    [Layer, SerializeField] private int _playerLayer;
    [SerializeField] private BoxCollider _boxTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == _playerLayer)
        {
            OnPlayerTriggerEnter?.Invoke();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == _playerLayer)
        {
            OnPlayerTriggerExit?.Invoke();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);

        Gizmos.DrawWireCube(_boxTrigger.center, _boxTrigger.size);
        Gizmos.matrix = oldMatrix;
    }
}
