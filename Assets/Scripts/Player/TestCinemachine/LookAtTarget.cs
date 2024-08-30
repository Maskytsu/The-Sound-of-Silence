using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _mainCamera;
    [SerializeField] private CinemachineVirtualCamera _lookAtCamera;
    [SerializeField] private CinemachineBrain _brain;
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _target;

    private PlayerMovementController _playerMovement;
    private bool _lookingAt = false;

    private void Awake()
    {
        _playerMovement = _player.GetComponent<PlayerMovementController>();
    }

    private void Update()
    {
        if(!_lookingAt && Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(LookAtTarget());
        }
    }

    private IEnumerator LookAtTarget()
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
