using Cinemachine;
using DG.Tweening;
using System.Collections;
using UnityEngine;

public class TESTLookAtObject : MonoBehaviour
{
    [SerializeField] private Transform _objectToLookAt;

    private Transform _playerCamera;
    private Transform _player;
    private PlayerMovement _playerMovement;
    private InputProvider _inputProvider;

    private bool _inAnimation = false;

    private void Start()
    {
        _playerCamera = PlayerManager.Instance.PlayerVirtualCamera.transform;
        _player = PlayerManager.Instance.Player.transform;
        _playerMovement = PlayerManager.Instance.PlayerMovemet;
        _inputProvider = InputProvider.Instance;
    }


    private void Update()
    {
        if (!_inAnimation && Input.GetKeyDown(KeyCode.Keypad2))
        {
            CameraManager.Instance.LookAtObject(_objectToLookAt);
            //StartCoroutine(Look());
        }
    }

    private IEnumerator Look()
    {
        _inAnimation = true;

        _inputProvider.TurnOffPlayerMaps();

        yield return null;

        Vector3 yRotationVector;
        Vector3 xRotationVector;

        CalculateRotationVectors(out yRotationVector, out xRotationVector);

        Tween rotationYTween = _player.DORotate(yRotationVector, 1.5f).SetEase(Ease.InOutSine);
        Tween rotationXTween = _playerCamera.DOLocalRotate(xRotationVector, 1.5f).SetEase(Ease.InOutSine);

        while (rotationYTween.IsPlaying() || rotationXTween.IsPlaying())
        {
            yield return null;
        }
        _playerMovement.SetXRotation(xRotationVector.x);

        yield return new WaitForSeconds(2f);

        _inputProvider.TurnOnPlayerMaps();
        yield return new WaitForEndOfFrame();

        _inAnimation = false;
    }

    private void CalculateRotationVectors(out Vector3 yRotationVector, out Vector3 xRotationVector)
    {
        Vector3 targetForwardVector = _objectToLookAt.position - _player.position;
        Quaternion targetLookRotation = Quaternion.LookRotation(targetForwardVector);

        yRotationVector = targetLookRotation.eulerAngles;
        yRotationVector.x = 0;

        if (targetLookRotation.eulerAngles.x > 180)
        {
            xRotationVector = Vector3.zero;
            xRotationVector.x = targetLookRotation.eulerAngles.x - 360f;
        }
        else
        {
            xRotationVector = Vector3.zero;
            xRotationVector.x = targetLookRotation.eulerAngles.x;
        }
    }
}
