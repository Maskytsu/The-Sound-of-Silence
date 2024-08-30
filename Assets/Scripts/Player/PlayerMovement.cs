using FMOD.Studio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Gravity Parameters")]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _pullingVelocity = 20f;
    [SerializeField] private LayerMask _groundMask;

    [Header("Movement Speed Parameters")]
    [SerializeField] private float _normalWalkSpeed = 3;
    [SerializeField] private float _slowWalkSpeed = 2.5f;
    [SerializeField] private float _normalSneakSpeed = 1.5f;
    [SerializeField] private float _slowSneakSpeed = 1.25f;

    [Header("Sensivity Parameters")]
    [SerializeField] private float _mouseSensivity = 15;

    [Header("Sneaking Parameters")]
    [SerializeField] private float _sneakHeight = 1.5f;
    [SerializeField] private float _standHeight = 3;
    [SerializeField] private float _timeToSneakStand = 0.35f;
    [SerializeField] private float _cameraOffset = 0.5f;
    [SerializeField] private Vector3 _sneakCenter = new Vector3(0, 0.75f, 0);
    [SerializeField] private Vector3 _standCenter = new Vector3(0, 0, 0);

    private Transform _mainCamera;
    private PlayerInputActions _playerInputActions;
    private CharacterController _characterController;
    private PlayerEquipment _playerEquipment;

    private float _xRotation = 0;
    private float _speed;
    private float _currentPullingVelocity;
    private bool _isGrounded;
    private bool _isSneaking = false;
    private bool _duringSneakStandAnimation = false;

    private EventInstance _playerFootsteps;


    private void Awake()
    {
        //assign proper values
        _mainCamera = PlayerManager.Instance.MainCamera.transform;
        _playerInputActions = new PlayerInputActions();
        _characterController = GetComponent<CharacterController>();
        _playerEquipment = GetComponent<PlayerEquipment>();

        //setup
        Cursor.lockState = CursorLockMode.Locked;
        _playerInputActions.PlayerMap.Enable();
        _speed = _normalWalkSpeed;
    }

    private void Start()
    {
        _playerFootsteps = AudioManager._instance.CreateEventInstance(FMODEvents.Instance.PlayerFootsteps);
    }

    private void Update()
    {
        RotateCharacter();
        ManageMovementSpeed();
        MoveCharacter();
        CreateGravity();
        ManageSneaking();
    }

    //------------------------------------------------------------------------------------------------------rotation and movement
    private void RotateCharacter()
    {
        //move up or down
        float mouseY = _playerInputActions.PlayerMap.MouseY.ReadValue<float>() * _mouseSensivity * Time.deltaTime;
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90, 90);
        _mainCamera.localRotation = Quaternion.Euler(_xRotation, 0, 0);

        //move left or right
        float mouseX = _playerInputActions.PlayerMap.MouseX.ReadValue<float>() * _mouseSensivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);
    }

    private void ManageMovementSpeed()
    {
        bool handsAreEmpty = _playerEquipment.HandsAreEmpty;
        if (!_isSneaking && handsAreEmpty) _speed = _normalWalkSpeed;
        else if (!_isSneaking && !handsAreEmpty) _speed = _slowWalkSpeed;
        else if (_isSneaking && handsAreEmpty) _speed = _normalSneakSpeed;
        else if (_isSneaking && !handsAreEmpty) _speed = _slowSneakSpeed;
    }

    private void MoveCharacter()
    {
        Vector2 inputVector = _playerInputActions.PlayerMap.Movement.ReadValue<Vector2>();
        Vector3 movement = transform.right * inputVector.x + transform.forward * inputVector.y;
        if(_isGrounded) _characterController.Move(movement * _speed * Time.deltaTime);
           
        if (inputVector != Vector2.zero && _isGrounded)
        {
            PLAYBACK_STATE playbackState;
            _playerFootsteps.getPlaybackState(out playbackState);
            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                _playerFootsteps.start();
            }
        }
        else
        {
            _playerFootsteps.stop(STOP_MODE.ALLOWFADEOUT);
        }
    }

    //------------------------------------------------------------------------------------------------------gravity
    private void CreateGravity()
    {
        _isGrounded = Physics.CheckSphere(_groundCheck.position, _characterController.radius, _groundMask);

        if(_isGrounded && _currentPullingVelocity > 2) //there is always some velocity for smoother transitions
        {
            _currentPullingVelocity = 2;
        }

        _currentPullingVelocity += _pullingVelocity * Time.deltaTime;
        _characterController.Move(Vector3.down * _currentPullingVelocity * Time.deltaTime); //it is doubly multiplied by time because of how physics equation for gravity works
    }

    //------------------------------------------------------------------------------------------------------sneaking
    private void ManageSneaking()
    {
        //check if changing state is needed
        if (!_duringSneakStandAnimation &&
            ((_playerInputActions.PlayerMap.Sneak.ReadValue<float>() > 0 && !_isSneaking)
            || (_playerInputActions.PlayerMap.Sneak.ReadValue<float>() == 0 && _isSneaking)))
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
        Vector3 characterTargetCenter = _isSneaking ? _standCenter : _sneakCenter;
        Vector3 characterCurrentCenter = _characterController.center;
        Vector3 groundCheckTargetPosition = _isSneaking ? 
            new Vector3(_groundCheck.localPosition.x, _groundCheck.localPosition.y - (_standHeight - _sneakHeight), _groundCheck.localPosition.z) :
            new Vector3(_groundCheck.localPosition.x, _groundCheck.localPosition.y + (_standHeight - _sneakHeight), _groundCheck.localPosition.z);
        Vector3 groundCheckCurrentPosition = _groundCheck.localPosition;

        //testing FMOD
        if (!_isSneaking) AudioManager._instance.PlayOneShot(FMODEvents.Instance.PlayerStartedSneaking, transform.position);

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

    //------------------------------------------------------------------------------------------------------enable/disable map
    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _playerInputActions.PlayerMap.Enable();
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.Confined;
        _playerInputActions.PlayerMap.Disable();
    }

    //------------------------------------------------------------------------------------------------------draw sneaking/stand up height Gizmo
    private void OnDrawGizmos()
    {
        if (_isSneaking)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(_mainCamera.position, Vector3.up * (_standHeight - _sneakHeight + _cameraOffset));
        }
    }
}
