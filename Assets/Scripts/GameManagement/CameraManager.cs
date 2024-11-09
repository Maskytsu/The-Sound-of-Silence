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
            _playerMovement = PlayerManager.Instance.PlayerMovement;
        }
    }

    public IEnumerator LookAtTargetAnimation(Transform target, float lookingAtTargetTime = 2f, float rotationTime = 1.5f)
    {
        if (_inAnimation)
        {
            Debug.LogError("Already in looking animation.");
            yield break;
        }

        _inAnimation = true;
        _inputProvider.TurnOffPlayerMaps();

        Vector3 newForwardVector = target.position - _player.position;
        Quaternion newRotation = Quaternion.LookRotation(newForwardVector);

        yield return StartCoroutine(_playerMovement.RotateCharacter(newRotation.eulerAngles, rotationTime));

        yield return new WaitForSeconds(lookingAtTargetTime);

        _inputProvider.TurnOnPlayerMaps();
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