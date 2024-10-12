using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Gravity Parameters")]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _pullingVelocity = 40f;
    [SerializeField] private LayerMask _groundMask;

    [Header("Movement Speed Parameters")]
    [SerializeField] private float _walkSpeed = 3.5f;
    [SerializeField] private float _sneakSpeed = 2.5f;

    [Header("Sensivity Parameters")]
    [SerializeField] private float _mouseSensivity = 8;

    [Header("Sneaking Parameters")]
    [SerializeField] private float _sneakHeight = 2f;
    [SerializeField] private float _standHeight = 3;
    [SerializeField] private float _timeToSneakStand = 0.35f;
    [SerializeField] private float _cameraOffset = 0.4f;
    [SerializeField] private Vector3 _sneakCenter = new Vector3(0, 0.5f, 0);

    private PlayerInputActions.PlayerKeyboardMapActions _playerKeyboardMap;
    private PlayerInputActions.PlayerMouseMapActions _playerMouseMap;
    private CharacterController _characterController;

    private Transform _mainCamera;
    private PlayerEquipment _playerEquipment;

    private float _slowWalkSpeed;
    private float _slowSneakSpeed;
    private float _xRotation = 0;
    private float _speed;
    private float _currentPullingVelocity;
    private bool _isGrounded;
    private bool _isSneaking = false;
    private bool _duringSneakStandAnimation = false;

    private EventInstance _playerFootsteps;


    private void Awake()
    {
        _slowWalkSpeed = _walkSpeed * 0.75f;
        _slowSneakSpeed = _sneakSpeed * 0.75f;
        _characterController = GetComponent<CharacterController>();
        _speed = _walkSpeed;
    }

    private void Start()
    {
        _playerKeyboardMap = InputProvider.Instance.PlayerKeyboardMap;
        _playerMouseMap = InputProvider.Instance.PlayerMouseMap;
        _mainCamera = PlayerManager.Instance.VirtualMainCamera.transform;
        _playerEquipment = PlayerManager.Instance.PlayerEquipment;
        _playerFootsteps = RuntimeManager.CreateInstance(FmodEvents.Instance.SFX_PlayerFootsteps);
    }

    private void Update()
    {
        RotateCharacter();
        ManageMovementSpeed();
        CheckIfGrounded();
        MoveCharacter();
        CreateGravity();
        ManageSneaking();
    }

    private void OnDrawGizmos()
    {
        if (_isSneaking)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(_mainCamera.position, Vector3.up * (_standHeight - _sneakHeight + _cameraOffset));
        }
    }

    public void SetXRotation(float rotation)
    {
        _xRotation = Mathf.Clamp(rotation, -90, 90);
        _mainCamera.localRotation = Quaternion.Euler(_xRotation, 0, 0);
    }

    private void RotateCharacter()
    {
        //move up or down
        float mouseY = _playerMouseMap.MouseY.ReadValue<float>() * _mouseSensivity * Time.deltaTime;
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90, 90);
        _mainCamera.localRotation = Quaternion.Euler(_xRotation, 0, 0);

        //move left or right
        float mouseX = _playerMouseMap.MouseX.ReadValue<float>() * _mouseSensivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);
    }

    private void ManageMovementSpeed()
    {
        bool handsAreEmpty = _playerEquipment.HandsAreEmpty;
        if (!_isSneaking && handsAreEmpty) _speed = _walkSpeed;
        else if (!_isSneaking && !handsAreEmpty) _speed = _slowWalkSpeed;
        else if (_isSneaking && handsAreEmpty) _speed = _sneakSpeed;
        else if (_isSneaking && !handsAreEmpty) _speed = _slowSneakSpeed;
    }

    private void CheckIfGrounded()
    {
        _isGrounded = Physics.CheckSphere(_groundCheck.position, _characterController.radius, _groundMask);
    }

    private void MoveCharacter()
    {
        Vector2 inputVector = _playerKeyboardMap.Movement.ReadValue<Vector2>();
        Vector3 movement = transform.right * inputVector.x + transform.forward * inputVector.y;
           
        if (inputVector != Vector2.zero && _isGrounded)
        {
            _characterController.Move(movement * _speed * Time.deltaTime);

            PLAYBACK_STATE playbackState;
            _playerFootsteps.getPlaybackState(out playbackState);
            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                _playerFootsteps.start();
            }
        }
        else
        {
            _playerFootsteps.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    private void CreateGravity()
    {
        if (_characterController.enabled)
        {
            //there is always some velocity (even if grounded) for smoother transitions
            if (_isGrounded && _currentPullingVelocity > 2)
            {
                _currentPullingVelocity = 2;
            }

            _currentPullingVelocity += _pullingVelocity * Time.deltaTime;
            //it is doubly multiplied by time because of how physics equation for gravity works
            _characterController.Move(Vector3.down * _currentPullingVelocity * Time.deltaTime);
        }
    }

    private void ManageSneaking()
    {
        //check if changing state is needed
        if (!_duringSneakStandAnimation &&
            ((_playerKeyboardMap.Sneak.ReadValue<float>() > 0 && !_isSneaking)
            || (_playerKeyboardMap.Sneak.ReadValue<float>() == 0 && _isSneaking)))
        {
            StartCoroutine(SneakingStanding());
        }
    }

    private IEnumerator SneakingStanding()
    {
        //check if standing up is possible
        float castDistance = _standHeight - _sneakHeight + _cameraOffset; //offset between cameras pos and top of character controller
        castDistance = castDistance - _characterController.radius;
        if (_isSneaking && Physics.SphereCast(_mainCamera.position, _characterController.radius, Vector3.up, out RaycastHit hitInfo, castDistance))
        {
            Debug.Log(hitInfo.transform.gameObject.name);
            yield break;
        }

        _duringSneakStandAnimation = true;

        float timeElapsed = 0;
        float characterTargetHeight = _isSneaking ? _standHeight : _sneakHeight;
        float characterCurrentHeight = _characterController.height;
        Vector3 characterTargetCenter = _isSneaking ? Vector3.zero : _sneakCenter;
        Vector3 characterCurrentCenter = _characterController.center;
        Vector3 groundCheckTargetPosition = _isSneaking ? 
            new Vector3(_groundCheck.localPosition.x, _groundCheck.localPosition.y - (_standHeight - _sneakHeight), _groundCheck.localPosition.z) :
            new Vector3(_groundCheck.localPosition.x, _groundCheck.localPosition.y + (_standHeight - _sneakHeight), _groundCheck.localPosition.z);
        Vector3 groundCheckCurrentPosition = _groundCheck.localPosition;

        //testing FMOD
        if (!_isSneaking) RuntimeManager.PlayOneShot(FmodEvents.Instance.SFX_PlayerStartedSneaking);

        //changing height and center of CharacterController in given time, also changing local position of ground check
        while (timeElapsed < _timeToSneakStand)
        {
            _characterController.height = Mathf.Lerp(characterCurrentHeight, characterTargetHeight, timeElapsed/ _timeToSneakStand);
            _characterController.center = Vector3.Lerp(characterCurrentCenter, characterTargetCenter, timeElapsed / _timeToSneakStand);
            _groundCheck.localPosition = Vector3.Lerp(groundCheckCurrentPosition, groundCheckTargetPosition, timeElapsed / _timeToSneakStand);
            if (!_isSneaking && _isGrounded) _characterController.Move(Vector3.down * _pullingVelocity); //pull down when CharacterControllers height is reduced
            timeElapsed  += Time.deltaTime;
            yield return null;
        }

        _characterController.height = characterTargetHeight;
        _characterController.center = characterTargetCenter;
        _groundCheck.localPosition = groundCheckTargetPosition;

        _isSneaking = !_isSneaking;
        _duringSneakStandAnimation = false;
    }
}
