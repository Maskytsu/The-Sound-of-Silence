using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionPrompt : MonoBehaviour
{
    private Transform mainCamera;

    private void Awake()
    {
        mainCamera = PlayerManager.Instance.MainCamera.transform;
    }

    void Update()
    {
        transform.forward = mainCamera.forward;
    }
}
