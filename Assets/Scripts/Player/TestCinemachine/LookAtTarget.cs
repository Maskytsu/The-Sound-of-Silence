using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    [SerializeField] private Transform _target;

    private CinemachineVirtualCamera _mainCamera;
    private CinemachineVirtualCamera _lookAtCamera;
    private CinemachineBrain _brain;
    private Transform _player;
    private PlayerMovement _playerMovement;

    private bool _lookingAt = false;

    private void Awake()
    {
        _mainCamera = PlayerManager.Instance.MainCamera;
        _lookAtCamera = PlayerManager.Instance.LookAtCamera;
        _brain = PlayerManager.Instance.CameraBrain;
        _player = PlayerManager.Instance.Player.transform;
        _playerMovement = PlayerManager.Instance.PlayerMovement;
    }

    private void Update()
    {
        if(!_lookingAt && Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(Look());
        }
    }

    private IEnumerator Look()
    {
        _lookingAt = true;

        _playerMovement.enabled = false;
        _lookAtCamera.LookAt = _target;

        _mainCamera.Priority = 1;
        _lookAtCamera.Priority = 9;

        yield return new WaitForSeconds(1f);
        while (_brain.IsBlending)
        {
            yield return new WaitForSeconds(0);
        }

        yield return new WaitForSeconds(2f);
        _mainCamera.Priority = 10;
        _lookAtCamera.Priority = 0;

        yield return new WaitForSeconds(1f);
        while (_brain.IsBlending)
        {
            yield return new WaitForSeconds(0);
        }

        _lookAtCamera.LookAt = null;
        _playerMovement.enabled = true;

        _lookingAt = false;
    }
}
