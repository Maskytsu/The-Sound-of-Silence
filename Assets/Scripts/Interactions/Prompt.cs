using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prompt : MonoBehaviour
{
    private Transform _playerCamera;

    private void Start()
    {
        _playerCamera = PlayerManager.Instance.PlayerVirtualCamera.transform;
    }

    void Update()
    {
        transform.forward = _playerCamera.forward;
    }
}
