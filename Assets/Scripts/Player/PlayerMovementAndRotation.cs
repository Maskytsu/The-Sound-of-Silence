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
    private bool _isCrouching = false;
    private bool _duringCrouchStandAnimation = false;

    private EventInstance _playerFootsteps;


    private void Awake()
    {
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

    private void ManageMovementSpeed()
    {
        bool handsAreEmpty = _playerEquipment.HandsAreEmpty;
        if (!_isCrouching && handsAreEmpty) _speed = _walkSpeed;
        else if (!_isCrouching && !handsAreEmpty) _speed = _slowWalkSpeed;
        else if (_isCrouching && handsAreEmpty) _speed = _crouchSpeed;
        else if (_isCrouching && !handsAreEmpty) _speed = _slowCrouchSpeed;
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
        //check if changing state is needed
        if (!_duringCrouchStandAnimation &&
            _playerMovementMap.Crouch.ReadValue<float>() != 0 
            && !_isCrouching)
        {
            StartCoroutine(Crouch());
        }

        if (!_duringCrouchStandAnimation &&
            _playerMovementMap.Crouch.ReadValue<float>() == 0
            && _isCrouching)
        {
            StartCoroutine(StandUp());
        }
    }

    private IEnumerator Crouch()
    {
        _duringCrouchStandAnimation = true;

        float timeElapsed = 0;
        float characterTargetHeight = _crouchHeight;
        float characterCurrentHeight = _characterController.height;
        Vector3 characterTargetCenter = _crouchCenter;
        Vector3 characterCurrentCenter = _characterController.center;
        Vector3 groundCheckTargetPosition = new Vector3(
            _groundCheck.localPosition.x, 
            _groundCheck.localPosition.y + (_standHeight - _crouchHeight), 
            _groundCheck.localPosition.z);
        Vector3 groundCheckCurrentPosition = _groundCheck.localPosition;

        //testing FMOD
        RuntimeManager.PlayOneShot(FmodEvents.Instance.SFX_PlayerStartedSneaking);

        //changing height and center of CharacterController in given time, also changing local position of ground check
        while (timeElapsed < _timeToCrouchStand)
        {
            _characterController.height = Mathf.Lerp(characterCurrentHeight, characterTargetHeight, timeElapsed/ _timeToCrouchStand);
            _characterController.center = Vector3.Lerp(characterCurrentCenter, characterTargetCenter, timeElapsed / _timeToCrouchStand);
            _groundCheck.localPosition = Vector3.Lerp(groundCheckCurrentPosition, groundCheckTargetPosition, timeElapsed / _timeToCrouchStand);
            if (_isGrounded) _characterController.Move(Vector3.down * _pullingVelocity); //pull down when CharacterControllers height is reduced
            timeElapsed  += Time.deltaTime;
            yield return null;
        }

        _characterController.height = characterTargetHeight;
        _characterController.center = characterTargetCenter;
        _groundCheck.localPosition = groundCheckTargetPosition;

        _isCrouching = !_isCrouching;
        _duringCrouchStandAnimation = false;
    }


    private IEnumerator StandUp()
    {
        //check if standing up is possible
        float castDistance = _standHeight - _crouchHeight + _cameraTopOffset; //offset between cameras pos and top of character controller
        castDistance = castDistance - _characterController.radius;
        if (_isCrouching && Physics.SphereCast(_mainCamera.position, _characterController.radius, Vector3.up, out RaycastHit hitInfo, castDistance))
        {
            yield return null;
            yield break;
        }

        _duringCrouchStandAnimation = true;

        float timeElapsed = 0;
        float characterTargetHeight = _standHeight;
        float characterCurrentHeight = _characterController.height;
        Vector3 characterTargetCenter = Vector3.zero;
        Vector3 characterCurrentCenter = _characterController.center;
        Vector3 groundCheckTargetPosition = new Vector3(
            _groundCheck.localPosition.x, 
            _groundCheck.localPosition.y - (_standHeight - _crouchHeight), 
            _groundCheck.localPosition.z);
        Vector3 groundCheckCurrentPosition = _groundCheck.localPosition;

        //changing height and center of CharacterController in given time, also changing local position of ground check
        while (timeElapsed < _timeToCrouchStand)
        {
            _characterController.height = Mathf.Lerp(characterCurrentHeight, characterTargetHeight, timeElapsed / _timeToCrouchStand);
            _characterController.center = Vector3.Lerp(characterCurrentCenter, characterTargetCenter, timeElapsed / _timeToCrouchStand);
            _groundCheck.localPosition = Vector3.Lerp(groundCheckCurrentPosition, groundCheckTargetPosition, timeElapsed / _timeToCrouchStand);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        _characterController.height = characterTargetHeight;
        _characterController.center = characterTargetCenter;
        _groundCheck.localPosition = groundCheckTargetPosition;

        _isCrouching = !_isCrouching;
        _duringCrouchStandAnimation = false;
    }
}
