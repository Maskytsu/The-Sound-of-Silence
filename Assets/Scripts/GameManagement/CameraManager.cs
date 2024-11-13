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
        if (PlayerObjectsHolder.Instance != null)
        {
            _playerCamera = PlayerObjectsHolder.Instance.PlayerVirtualCamera.transform;
            _player = PlayerObjectsHolder.Instance.Player.transform;
            _playerMovement = PlayerObjectsHolder.Instance.PlayerMovement;
        }
    }

    public IEnumerator LookAtTargetAnimation(Transform target, float rotationTime = 1.5f, float lookingAtTargetTime = 2f)
    {
        if (_inAnimation)
        {
            Debug.LogError("Already in looking animation.");
            yield break;
        }

        _inAnimation = true;
        _inputProvider.TurnOffPlayerMaps();

        Vector3 newForwardVector = target.position - _playerCamera.position;
        Quaternion newRotation = Quaternion.LookRotation(newForwardVector);

        yield return StartCoroutine(_playerMovement.RotateCharacter(newRotation.eulerAngles, rotationTime));

        yield return new WaitForSeconds(lookingAtTargetTime);

        _inAnimation = false;
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