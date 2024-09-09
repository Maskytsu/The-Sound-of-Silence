using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    [SerializeField] private Transform _target;

    private CinemachineVirtualCamera _mainCamera;
    private CinemachineVirtualCamera _lookAtCamera;
    private CinemachineBrain _cameraBrain;
    private Transform _player;
    private PlayerMovement _playerMovement;

    private bool _lookingAt = false;

    private void Awake()
    {
        _mainCamera = PlayerManager.Instance.MainCamera;
        _lookAtCamera = PlayerManager.Instance.LookAtCamera;
        _cameraBrain = PlayerManager.Instance.CameraBrain;
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
                _lookAtCamera.LookAt = _target;

        _playerMovement.enabled = false;

        _mainCamera.Priority = 1;
        _lookAtCamera.Priority = 9;

        yield return new WaitForSeconds(1f);
        while (_cameraBrain.IsBlending)
        {
            yield return new WaitForSeconds(0);
        }

        yield return new WaitForSeconds(2f);

        MainCameraToTarget();
        _playerMovement.enabled = true;

        _mainCamera.Priority = 10;
        _lookAtCamera.Priority = 0;

        yield return new WaitForEndOfFrame();

        _lookAtCamera.LookAt = null;
        _lookingAt = false;
    }

    private void MainCameraToTarget()
    {
        Vector3 lookPos = _target.position - _player.position;
        lookPos.y = 0;
        _player.rotation = Quaternion.LookRotation(lookPos);

        if (_lookAtCamera.transform.localEulerAngles.x > 180)
        {
            _playerMovement.SetXRotation(_lookAtCamera.transform.localEulerAngles.x - 360f);
        }
        else
        {
            _playerMovement.SetXRotation(_lookAtCamera.transform.localEulerAngles.x);
        }
 
    }
}
