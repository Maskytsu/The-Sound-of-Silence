using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTriggerChild : MonoBehaviour
{
    [SerializeField] private PlayerTrigger _playerTriggerParent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == _playerTriggerParent.PlayerLayer)
        {
            _playerTriggerParent.OnPlayerTriggerEnter?.Invoke();
            gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        BoxCollider boxTrigger = GetComponent<BoxCollider>();

        Gizmos.color = Color.magenta;
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);

        Gizmos.DrawWireCube(boxTrigger.center, boxTrigger.size);
        Gizmos.matrix = oldMatrix;
    }
}
