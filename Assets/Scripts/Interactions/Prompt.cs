using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prompt : MonoBehaviour
{
    private Transform _playerCamera;

    private void Awake()
    {
        transform.localScale = new Vector3(
            transform.localScale.x * (transform.lossyScale.x < 0 ? -1.0f : 1.0f),
            transform.localScale.y * (transform.lossyScale.y < 0 ? -1.0f : 1.0f),
            transform.localScale.z * (transform.lossyScale.z < 0 ? -1.0f : 1.0f));
    }

    private void Start()
    {
        _playerCamera = PlayerObjects.Instance.PlayerVirtualCamera.transform;
    }

    void Update()
    {
        transform.forward = _playerCamera.forward;
    }
}
