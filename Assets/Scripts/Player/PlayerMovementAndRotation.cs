using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using System.Collections;
using UnityEngine;

public class PlayerMovementAndRotation : MonoBehaviour
{
    [Header("Gravity Parameters")]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _pullingVelocity = 40f;
    [SerializeField] private LayerMask _groundMask;

    [Header("Movement Speed Parameters")]
    [SerializeField] private float _walkSpeed = 2.5f;
    [SerializeField] private float _crouchSpeed = 1.5f;

    [Header("Sensivity Parameters")]
    [SerializeField] private float _mouseSensivity = 8;

    [Header("Crouching Parameters")]
    [SerializeField] private float _crouchHeight = 1.5f;
    [SerializeField] private Vector3 _crouchCenter = new Vector3(0, 0.75f, 0);
    [SerializeField] private float _standHeight = 3;
    [SerializeField] private float _timeToCrouchStand = 0.5f;
    [SerializeField] private float _cameraTopOffset = 0.4f;

    private PlayerInputActions.PlayerMovementMapActions _playerMovementMap;
    private PlayerInputActions.PlayerMainMapActions _playerMainMap;
    private CharacterController _characterController;

    private Transform _mainCamera;
    private PlayerEquipment _playerEquipment;

    private float _slowWalkSpeed;
    private float _slowCrouchSpeed;
    private float _xRotation = 0;
    private float _speed;
    private float _currentPullingVelocity;
    private bool _isGrounded;
    private Coroutine _crouchCoroutine;
    private Coroutine _standUpCoroutine;
    private bool _isCrouching = false;
    private float _animationTimeElapsed;

    private EventInstance _playerFootsteps;


    private void Awake()
    {
        _animationTimeElapsed = _timeToCrouchStand;
        _slowWalkSpeed = _walkSpeed * 0.75f;
        _slowCrouchSpeed = _crouchSpeed * 0.75f;
        _characterController = GetComponent<CharacterController>();
        _speed = _walkSpeed;
    }

    private void Start()
    {
        _playerMovementMap = InputProvider.Instance.PlayerMovementMap;
        _playerMainMap = InputProvider.Instance.PlayerMainMap;
        _mainCamera = PlayerManager.Instance.VirtualMainCamera.transform;
        _playerEquipment = PlayerManager.Instance.PlayerEquipment;
        _playerFootsteps = RuntimeManager.CreateInstance(FmodEvents.Instance.SFX_PlayerFootsteps);
    }

    private void Update()
    {
        RotateCharacter();
        ManageMovementSpeed();
        CheckIfGrounded();
        ManageMovement();
        CreateGravity();
        ManageCrouching();
    }

    private void OnDrawGizmos()
    {
        if (_isCrouching)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(_mainCamera.position, Vector3.up * (_standHeight - _crouchHeight + _cameraTopOffset));
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
        float mouseY = _playerMainMap.MouseY.ReadValue<float>() * _mouseSensivity * Time.deltaTime;
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90, 90);
        _mainCamera.localRotation = Quaternion.Euler(_xRotation, 0, 0);

        //move left or right
        float mouseX = _playerMainMap.MouseX.ReadValue<float>() * _mouseSensivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);
    }

    private bool IsCrouchingOrInBetween => _isCrouching || _crouchCoroutine != null || _standUpCoroutine != null;

    private void ManageMovementSpeed()
    {
        bool handsAreEmpty = _playerEquipment.HandsAreEmpty;
        if (!IsCrouchingOrInBetween && handsAreEmpty) _speed = _walkSpeed;
        else if (!IsCrouchingOrInBetween && !handsAreEmpty) _speed = _slowWalkSpeed;
        else if (IsCrouchingOrInBetween && handsAreEmpty) _speed = _crouchSpeed;
        else if (IsCrouchingOrInBetween && !handsAreEmpty) _speed = _slowCrouchSpeed;
    }

    private void CheckIfGrounded()
    {
        _isGrounded = Physics.CheckSphere(_groundCheck.position, _characterController.radius, _groundMask);
    }

    private void ManageMovement()
    {
        Vector2 inputVector = _playerMovementMap.Movement.ReadValue<Vector2>();
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

    private void ManageCrouching()
    {
        if (!_isCrouching && _playerMovementMap.Crouch.ReadValue<float>() > 0)
        {
            if (_standUpCoroutine != null)
            {
                StopCoroutine(_standUpCoroutine);
                _standUpCoroutine = null;
            }

            if (_crouchCoroutine == null)
            {
                _crouchCoroutine = StartCoroutine(Crouch());
                _isCrouching = true;
            }
        }


        if (_isCrouching && _playerMovementMap.Crouch.ReadValue<float>() == 0 && CheckIfCanStandUp())
        {
            if (_crouchCoroutine != null)
            {
                StopCoroutine(_crouchCoroutine);
                _crouchCoroutine = null;
            }

            if (_standUpCoroutine == null)
            {
                _standUpCoroutine = StartCoroutine(StandUp());
                _isCrouching = false;
            }
        }
    }

    private IEnumerator Crouch()
    {
        _animationTimeElapsed = _timeToCrouchStand - _animationTimeElapsed;
        float targetHeight = _crouchHeight;
        float currentHeight = _characterController.height;
        Vector3 targetCenter = _crouchCenter;
        Vector3 currentCenter = _characterController.center;
        Vector3 targetGroundCheckPos = new Vector3(
            _groundCheck.localPosition.x,
            -(_standHeight / 2) + (_standHeight - _crouchHeight),
            _groundCheck.localPosition.z);
        Vector3 currentGroundCheckPos = _groundCheck.localPosition;

        float currentPositionY = transform.position.y;
        float targetPositionY = _groundCheck.position.y - (_standHeight / 2 - _crouchHeight);

        //changing height and center of CharacterController in given time, also changing local position of ground check
        while (_animationTimeElapsed < _timeToCrouchStand)
        {
            _characterController.height = Mathf.Lerp(currentHeight, targetHeight, _animationTimeElapsed/ _timeToCrouchStand);
            _characterController.center = Vector3.Lerp(currentCenter, targetCenter, _animationTimeElapsed / _timeToCrouchStand);
            _groundCheck.localPosition = Vector3.Lerp(currentGroundCheckPos, targetGroundCheckPos, _animationTimeElapsed / _timeToCrouchStand);

            float posY = Mathf.Lerp(currentPositionY, targetPositionY, _animationTimeElapsed / _timeToCrouchStand);
            transform.position = new Vector3(transform.position.x, posY, transform.position.z);

            _animationTimeElapsed  += Time.deltaTime;
            yield return null;
        }

        _animationTimeElapsed = _timeToCrouchStand;
        _characterController.height = targetHeight;
        _characterController.center = targetCenter;
        _groundCheck.localPosition = targetGroundCheckPos;

        transform.position = new Vector3(transform.position.x, targetPositionY, transform.position.z);

        _crouchCoroutine = null;
    }


    private IEnumerator StandUp()
    {
        _animationTimeElapsed = _timeToCrouchStand - _animationTimeElapsed;
        float targetHeight = _standHeight;
        float currentHeight = _characterController.height;
        Vector3 targetCenter = Vector3.zero;
        Vector3 currentCenter = _characterController.center;
        Vector3 targetGoundCheckPosition = new Vector3(
            _groundCheck.localPosition.x,
            -(_standHeight / 2),
            _groundCheck.localPosition.z);
        Vector3 currentGroundCheckPosition = _groundCheck.localPosition;

        float currentPositionY = transform.position.y;
        float targetPositionY = _groundCheck.position.y + (_standHeight / 2);

        //changing height and center of CharacterController in given time, also changing local position of ground check
        while (_animationTimeElapsed < _timeToCrouchStand)
        {
            _characterController.height = Mathf.Lerp(currentHeight, targetHeight, _animationTimeElapsed / _timeToCrouchStand);
            _characterController.center = Vector3.Lerp(currentCenter, targetCenter, _animationTimeElapsed / _timeToCrouchStand);
            _groundCheck.localPosition = Vector3.Lerp(currentGroundCheckPosition, targetGoundCheckPosition, _animationTimeElapsed / _timeToCrouchStand);

            float posY = Mathf.Lerp(currentPositionY, targetPositionY, _animationTimeElapsed / _timeToCrouchStand);
            transform.position = new Vector3(transform.position.x, posY, transform.position.z);

            _animationTimeElapsed += Time.deltaTime;
            yield return null;
        }

        _animationTimeElapsed = _timeToCrouchStand;
        _characterController.height = targetHeight;
        _characterController.center = targetCenter;
        _groundCheck.localPosition = targetGoundCheckPosition;

        transform.position = new Vector3(transform.position.x, targetPositionY, transform.position.z);

        _standUpCoroutine = null;
    }

    private bool CheckIfCanStandUp()
    {
        //offset between cameras pos and top of character controller
        float castDistance = _standHeight - _crouchHeight + _cameraTopOffset;
        castDistance = castDistance - _characterController.radius;

        if (Physics.SphereCast(_mainCamera.position, _characterController.radius, Vector3.up, out RaycastHit hitInfo, castDistance))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
