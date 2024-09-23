using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prompt : MonoBehaviour
{
    private Transform _mainCamera;

    private void Start()
    {
        _mainCamera = PlayerManager.Instance.VirtualMainCamera.transform;
    }

    void Update()
    {
        transform.forward = _mainCamera.forward;
    }
}
