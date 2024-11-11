using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Objects")]
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Transform _playerCamera;

    [Header("Gravity Parameters")]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _pullingVelocity = 40f;
    [SerializeField] private LayerMask _groundMask;

    [Header("Movement Speed Parameters")]
    [SerializeField] private float _walkSpeed = 2.5f;
    [SerializeField] private float _crouchSpeed = 1.5f;

    [Header("Sensivity Parameters")]
    [SerializeField] private float _mouseSensivity = 8f;

    [Header("Crouching Parameters")]
    [SerializeField] private float _crouchHeight = 1.5f;
    [SerializeField] private float _timeBetweenHeights = 0.6f;
    [Tooltip("Offset between camera position and top of the character controller")]
    [SerializeField] private float _cameraTopOffset = 0.4f;
    [SerializeField] private LayerMask _stairsMask;

    private PlayerInputActions.PlayerMovementMapActions _playerMovementMap;
    private PlayerInputActions.PlayerMainMapActions _playerMainMap;

    private Transform _player;
    private PlayerEquipment _playerEquipment;

    private float _standHeight;
    private float _slowWalkSpeed;
    private float _slowCrouchSpeed;
    private float _speed;
    private float _currentXRotation;
    private bool _inRotateAnimation = false;
    private float _currentPullingVelocity;
    private Coroutine _crouchCoroutine;
    private Coroutine _standUpCoroutine;
    private bool _isCrouching = false;
    private float _crouchAnimationTime;

    private EventInstance _playerFootsteps;

    private bool IsCrouchingOrInBetween => _isCrouching || _crouchCoroutine != null || _standUpCoroutine != null;
    private bool IsGrounded => Physics.CheckSphere(_groundCheck.position, _characterController.radius, _groundMask);
    private bool IsOnStairs => Physics.CheckSphere(_groundCheck.position, _characterController.radius, _stairsMask, QueryTriggerInteraction.Collide);

    private void Awake()
    {
        _standHeight = _characterController.height;
        _slowWalkSpeed = _walkSpeed * 0.75f;
        _slowCrouchSpeed = _crouchSpeed * 0.75f;
        _speed = _walkSpeed;
        _currentXRotation = XRotationVectorFromEulers(_playerCamera.localEulerAngles).x;
    }

    private void Start()
    {
        _player = PlayerManager.Instance.Player.transform;
        _playerMovementMap = InputProvider.Instance.PlayerMovementMap;
        _playerMainMap = InputProvider.Instance.PlayerMainMap;
        _playerCamera = PlayerManager.Instance.PlayerVirtualCamera.transform;
        _playerEquipment = PlayerManager.Instance.PlayerEquipment;
        _playerFootsteps = RuntimeManager.CreateInstance(FmodEvents.Instance.SFX_PlayerFootsteps);
    }

    private void Update()
    {
        ManageMouseRotation();
        CalculateMovementSpeed();
        ManageMovement();
        CreateGravity();
        ManageCrouching();
    }

    private void OnDrawGizmos()
    {
        if (_isCrouching)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(_playerCamera.position, Vector3.up * (_cameraTopOffset + _standHeight - _crouchHeight));
        }
    }

    public IEnumerator RotateCharacter(Vector3 rotation, float rotationTime)
    {
        if (_inRotateAnimation)
        {
            Debug.LogError("Already rotating.");
            yield break;
        }

        _inRotateAnimation = true;

        Vector3 yRotationVector = rotation;
        yRotationVector.x = 0;

        Vector3 xRotationVector = XRotationVectorFromEulers(rotation);

        if (rotationTime != 0)
        {
            Tween rotationYTween = _player.DORotate(yRotationVector, rotationTime).SetEase(Ease.InOutSine);
            Tween rotationXTween = _playerCamera.DOLocalRotate(xRotationVector, rotationTime).SetEase(Ease.InOutSine);

            while (rotationYTween.IsPlaying() || rotationXTween.IsPlaying())
            {
                yield return null;
            }
        }
        else
        {
            _player.rotation = Quaternion.Euler(yRotationVector);
            _playerCamera.localRotation = Quaternion.Euler(xRotationVector);
        }

        _currentXRotation = Mathf.Clamp(xRotationVector.x, -90f, 90f);
        _playerCamera.localRotation = Quaternion.Euler(_currentXRotation, 0, 0);

        _inRotateAnimation = false;
    }

    private Vector3 XRotationVectorFromEulers(Vector3 rotation)
    {
        Vector3 xRotationVector;

        //eulers are (0)-(360)a but regular rotation is (0)-(180) and (-180)-(0)
        if (rotation.x > 180)
        {
            xRotationVector = Vector3.zero;
            xRotationVector.x = rotation.x - 360f;
        }
        else
        {
            xRotationVector = Vector3.zero;
            xRotationVector.x = rotation.x;
        }

        return xRotationVector;
    }

    private void ManageMouseRotation()
    {
        //move camera up or down
        float mouseY = _playerMainMap.MouseY.ReadValue<float>() * _mouseSensivity * Time.deltaTime;
        _currentXRotation -= mouseY;
        _currentXRotation = Mathf.Clamp(_currentXRotation, -90, 90);
        _playerCamera.localRotation = Quaternion.Euler(_currentXRotation, 0, 0);

        //rotate whole player object left or right
        float mouseX = _playerMainMap.MouseX.ReadValue<float>() * _mouseSensivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);
    }

    private void CalculateMovementSpeed()
    {
        bool handsAreEmpty = _playerEquipment.HandsAreEmpty;

        if (!IsCrouchingOrInBetween && handsAreEmpty) _speed = _walkSpeed;
        else if (!IsCrouchingOrInBetween && !handsAreEmpty) _speed = _slowWalkSpeed;
        else if (IsCrouchingOrInBetween && handsAreEmpty) _speed = _crouchSpeed;
        else if (IsCrouchingOrInBetween && !handsAreEmpty) _speed = _slowCrouchSpeed;
    }

    private void ManageMovement()
    {
        Vector2 inputVector = _playerMovementMap.Movement.ReadValue<Vector2>();
        Vector3 movement = transform.right * inputVector.x + transform.forward * inputVector.y;
           
        if (inputVector != Vector2.zero && IsGrounded)
        {
            _characterController.Move(movement * _speed * Time.deltaTime);

            _playerFootsteps.getPlaybackState(out PLAYBACK_STATE playbackState);
            if (playbackState.Equals(PLAYBACK_STATE.STOPPED)) _playerFootsteps.start();
        }
    }

    private void CreateGravity()
    {
        if (_characterController.enabled)
        {
            //there is always some velocity (even if grounded) for smoother transitions
            if (IsGrounded && _currentPullingVelocity > 2)
            {
                _currentPullingVelocity = 2;
            }

            //there is double multiply by time because of how physics equation for gravity works
            _currentPullingVelocity += _pullingVelocity * Time.deltaTime;
            _characterController.Move(Vector3.down * _currentPullingVelocity * Time.deltaTime);
        }
    }

    private void ManageCrouching()
    {
        //crouch
        if (!_isCrouching && !IsOnStairs && _playerMovementMap.Crouch.ReadValue<float>() > 0)
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

        //stand up (if input or approaching stairs)
        if ((_isCrouching && _playerMovementMap.Crouch.ReadValue<float>() == 0 && CheckIfCanStandUp())
            || (_isCrouching && IsOnStairs))
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
        float timeElpased = 0f;

        float targetHeight = _crouchHeight;
        float currentHeight = _characterController.height;

        //calculations in image file
        Vector3 targetCenter = new Vector3(0, (_standHeight / 2) - (_crouchHeight / 2), 0);
        Vector3 currentCenter = _characterController.center;

        //-(_standHeight / 2) is bottom of character when standing
        //(_standHeight - _crouchHeight) is difference between heights so also between bottoms
        //targetPos.y = (bottom when standing) + (difference beetween heights) = (bottom when crouching)
        Vector3 targetGroundCheckPos = new Vector3(
            _groundCheck.localPosition.x,
            -(_standHeight / 2) + (_standHeight - _crouchHeight), 
            _groundCheck.localPosition.z);
        Vector3 currentGroundCheckPos = _groundCheck.localPosition;

        //calculations in image file
        float targetPositionY = _groundCheck.position.y - (_standHeight / 2 - _crouchHeight);
        float currentPositionY = transform.position.y;

        //calculations in image file
        _crouchAnimationTime = (_timeBetweenHeights * (currentHeight - _crouchHeight)) / (_standHeight - _crouchHeight);

        while (timeElpased < _crouchAnimationTime)
        {
            _characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElpased / _crouchAnimationTime);
            _characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElpased / _crouchAnimationTime);
            _groundCheck.localPosition = Vector3.Lerp(currentGroundCheckPos, targetGroundCheckPos, timeElpased / _crouchAnimationTime);
            float positionY = Mathf.Lerp(currentPositionY, targetPositionY, timeElpased / _crouchAnimationTime);
            transform.position = new Vector3(transform.position.x, positionY, transform.position.z);

            timeElpased += Time.deltaTime;
            yield return null;
        }

        _characterController.height = targetHeight;
        _characterController.center = targetCenter;
        _groundCheck.localPosition = targetGroundCheckPos;
        transform.position = new Vector3(transform.position.x, targetPositionY, transform.position.z);

        _crouchCoroutine = null;
    }

    private IEnumerator StandUp()
    {
        float timeElpased = 0f;

        float targetHeight = _standHeight;
        float currentHeight = _characterController.height;

        Vector3 targetCenter = Vector3.zero;
        Vector3 currentCenter = _characterController.center;

        //-(_standHeight / 2) is bottom of character when standing
        Vector3 targetGoundCheckPosition = new Vector3(
            _groundCheck.localPosition.x,
            -(_standHeight / 2),
            _groundCheck.localPosition.z);
        Vector3 currentGroundCheckPosition = _groundCheck.localPosition;

        //calculations in image file
        float targetPositionY = _groundCheck.position.y + (_standHeight / 2);
        float currentPositionY = transform.position.y;

        //calculations in image file
        _crouchAnimationTime = (_timeBetweenHeights * (_standHeight - currentHeight)) / (_standHeight - _crouchHeight);

        while (timeElpased < _crouchAnimationTime)
        {
            _characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElpased / _crouchAnimationTime);
            _characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElpased / _crouchAnimationTime);
            _groundCheck.localPosition = Vector3.Lerp(currentGroundCheckPosition, targetGoundCheckPosition, timeElpased / _crouchAnimationTime);
            float posY = Mathf.Lerp(currentPositionY, targetPositionY, timeElpased / _crouchAnimationTime);
            transform.position = new Vector3(transform.position.x, posY, transform.position.z);

            timeElpased += Time.deltaTime;
            yield return null;
        }

        _characterController.height = targetHeight;
        _characterController.center = targetCenter;
        _groundCheck.localPosition = targetGoundCheckPosition;
        transform.position = new Vector3(transform.position.x, targetPositionY, transform.position.z);

        _standUpCoroutine = null;
    }

    private bool CheckIfCanStandUp()
    {
        //starts from camera but we need to start from top so +offset
        float castDistance = _cameraTopOffset;
        //radius is inside of the sphereCast so we don't need it here
        castDistance -= _characterController.radius;
        //+height that is requierd to stand up
        castDistance += _standHeight - _crouchHeight;

        RaycastHit[] hits = Physics.SphereCastAll(_playerCamera.position, _characterController.radius, Vector3.up, castDistance);

        //if (Physics.SphereCast(_playerCamera.position, _characterController.radius, Vector3.up, out RaycastHit hitInfo, castDistance))
        if (hits.Length > 1)
        {
            return false;
        }

        return true;
    }
}
