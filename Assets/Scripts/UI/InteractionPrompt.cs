using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionPrompt : MonoBehaviour
{
    private Transform _mainCamera;
    private void Awake()
    {
        _mainCamera = PlayerManager.Instance.MainCamera.transform;
    }
    void Update()
    {
        transform.forward = _mainCamera.forward;
    }
}
