using NaughtyAttributes;
using System;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    public Action OnPlayerTriggerEnter;
    public Action OnPlayerTriggerExit;

    [Layer, SerializeField] private int _playerLayer;

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
}
