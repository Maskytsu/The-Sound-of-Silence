using Cinemachine;
using DG.Tweening;
using System.Collections;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    public CinemachineBrain CameraBrain;
    public Camera MainCamera;
    public Camera PhoneInteractCamera;
    [Space]
    [SerializeField] private InputProvider _inputProvider;

    private Transform _playerCamera;
    private Transform _player;
    private PlayerMovement _playerMovement;

    private bool _inAnimation = false;

    private void Awake()
    {
        CreateInstance();
    }

    private void Start()
    {
        if (PlayerManager.Instance != null)
        {
            _playerCamera = PlayerManager.Instance.PlayerVirtualCamera.transform;
            _player = PlayerManager.Instance.Player.transform;
            _playerMovement = PlayerManager.Instance.PlayerMovemet;
        }
    }

    public void LookAtObject(Transform target)
    {
        StartCoroutine(LookAtTargetAnimation(target));
    }

    private IEnumerator LookAtTargetAnimation(Transform target)
    {
        if (_inAnimation)
        {
            Debug.LogError("Already in looking animation.");
            yield break;
        }

        _inAnimation = true;
        _inputProvider.TurnOffPlayerMaps();

        Vector3 yRotationVector;
        Vector3 xRotationVector;
        CalculateRotationVectors(target, out yRotationVector, out xRotationVector);

        Tween rotationYTween = _player.DORotate(yRotationVector, 1.5f).SetEase(Ease.InOutSine);
        Tween rotationXTween = _playerCamera.DOLocalRotate(xRotationVector, 1.5f).SetEase(Ease.InOutSine);

        while (rotationYTween.IsPlaying() || rotationXTween.IsPlaying())
        {
            yield return null;
        }

        _playerMovement.SetXRotation(xRotationVector.x);
        yield return new WaitForSeconds(2f);

        _inputProvider.TurnOnPlayerMaps();
        _inAnimation = false;
    }

    private void CalculateRotationVectors(Transform target, out Vector3 yRotationVector, out Vector3 xRotationVector)
    {
        Vector3 newForwardVector = target.position - _player.position;
        Quaternion newRotation = Quaternion.LookRotation(newForwardVector);

        yRotationVector = newRotation.eulerAngles;
        yRotationVector.x = 0;

        if (newRotation.eulerAngles.x > 180)
        {
            xRotationVector = Vector3.zero;
            xRotationVector.x = newRotation.eulerAngles.x - 360f;
        }
        else
        {
            xRotationVector = Vector3.zero;
            xRotationVector.x = newRotation.eulerAngles.x;
        }
    }

    private void CreateInstance()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one CameraManager in the scene.");
        }
        Instance = this;
    }
}