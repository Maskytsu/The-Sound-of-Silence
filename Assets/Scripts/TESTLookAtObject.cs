using Cinemachine;
using DG.Tweening;
using System.Collections;
using UnityEngine;

public class TESTLookAtObject : MonoBehaviour
{
    [SerializeField] private Transform _objectToLookAt;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            StartCoroutine(CameraManager.Instance.LookAtTargetAnimation(_objectToLookAt));
        }
    }
}