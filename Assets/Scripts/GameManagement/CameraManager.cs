using Cinemachine;
using DG.Tweening;
using System.Collections;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [field: SerializeField] public CinemachineBrain CameraBrain { get; private set; }
    [field: SerializeField] public Camera MainCamera { get; private set; }
    [field: SerializeField] public Camera PhoneInteractCamera { get; private set; }
    [Space]
    [SerializeField] private InputProvider _inputProvider;

    private Transform _playerCamera;
    private PlayerMovement _playerMovement;

    private bool _inLookingAnimation = false;

    private void Awake()
    {
        CreateInstance();
    }

    private void Start()
    {
        if (PlayerObjects.Instance != null)
        {
            _playerCamera = PlayerObjects.Instance.PlayerVirtualCamera.transform;
            _playerMovement = PlayerObjects.Instance.PlayerMovement;
        }
    }

    public IEnumerator LookAtTargetAnimation(Transform target, float rotationTime = 1.5f, float lookingAtTargetTime = 2f)
    {
        if (_inLookingAnimation)
        {
            Debug.LogError("Already in looking animation.");
            yield break;
        }

        _inLookingAnimation = true;
        _inputProvider.TurnOffPlayerMaps();

        Vector3 newForwardVector = target.position - _playerCamera.position;
        Quaternion newRotation = Quaternion.LookRotation(newForwardVector);

        yield return StartCoroutine(_playerMovement.RotateCharacterAnimation(newRotation.eulerAngles, rotationTime));

        yield return new WaitForSeconds(lookingAtTargetTime);

        _inLookingAnimation = false;
    }

    public IEnumerator LookAtTargetAnimation(Vector3 targetPos, float rotationTime = 1.5f, float lookingAtTargetTime = 2f)
    {
        if (_inLookingAnimation)
        {
            Debug.LogError("Already in looking animation.");
            yield break;
        }

        _inLookingAnimation = true;
        _inputProvider.TurnOffPlayerMaps();

        Vector3 newForwardVector = targetPos - _playerCamera.position;
        Quaternion newRotation = Quaternion.LookRotation(newForwardVector);

        yield return StartCoroutine(_playerMovement.RotateCharacterAnimation(newRotation.eulerAngles, rotationTime));

        yield return new WaitForSeconds(lookingAtTargetTime);

        _inLookingAnimation = false;
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