using Cinemachine;
using System.Collections;
using UnityEngine;

public class TESTLookAtTarget : MonoBehaviour
{
    [SerializeField] private Transform _target;

    private CinemachineVirtualCamera _mainCamera;
    private CinemachineVirtualCamera _lookAtCamera;
    private CinemachineBrain _cameraBrain;
    private Transform _player;
    private PlayerMovement _playerMovement;
    private InputProvider _inputProvider;

    private bool _lookingAt = false;

    private void Start()
    {
        _mainCamera = PlayerManager.Instance.VirtualMainCamera;
        _lookAtCamera = PlayerManager.Instance.VirtualLookAtCamera;
        _cameraBrain = PlayerManager.Instance.CameraBrain;
        _player = PlayerManager.Instance.Player.transform;
        _playerMovement = PlayerManager.Instance.PlayerMovement;
        _inputProvider = InputProvider.Instance;
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

        _inputProvider.TurnOffPlayerMap();

        _mainCamera.enabled = false;

        yield return new WaitForSeconds(1f);
        while (_cameraBrain.IsBlending)
        {
            yield return new WaitForSeconds(0);
        }

        yield return new WaitForSeconds(2f);

        MainCameraToTarget();
        _inputProvider.TurnOnPlayerMap();

        _mainCamera.enabled = true;

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
