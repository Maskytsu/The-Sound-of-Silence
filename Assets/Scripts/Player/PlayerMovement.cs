using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using NaughtyAttributes;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [ShowNativeProperty] public bool IsHidding => Application.isPlaying && !CanStandUp();

    [Header("Player Objects")]
    [SerializeField] private Transform _player;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Transform _playerCamera;
    [SerializeField] private PlayerEquipment _playerEquipment;
    [SerializeField] private Transform _groundCheck;

    [Header("Gravity Parameters")]
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
    [SerializeField] private EventReference _playerFootstepsRef;

    [Header("Debugging")]
    [SerializeField] private bool _debugEulerAngles = false;

    private float _standHeight;
    private float _slowWalkSpeed;
    private float _slowCrouchSpeed;
    private float _speed;
    private float _currentXRotation;
    private bool _inMoveAnimation = false;
    private bool _inRotateAnimation = false;
    private float _currentPullingVelocity;
    private Coroutine _crouchCoroutine;
    private Coroutine _standUpCoroutine;
    private bool _isCrouching = false;
    private float _crouchAnimationTime;

    private EventInstance _playerFootsteps;

    private PlayerInputActions.PlayerMovementMapActions PlayerMovementMap => InputProvider.Instance.PlayerMovementMap;
    private PlayerInputActions.PlayerCameraMapActions PlayerCameraMap => InputProvider.Instance.PlayerCameraMap;

    private bool IsCrouchingOrInBetween => _isCrouching || _crouchCoroutine != null || _standUpCoroutine != null;
    //adding + 0.2f for better walking on stair slopes 
    private bool IsGrounded => Physics.CheckSphere(_groundCheck.position, _characterController.radius + 0.2f, _groundMask);
    private bool IsOnStairs => Physics.CheckSphere(_groundCheck.position, _characterController.radius + 0.2f, _stairsMask, QueryTriggerInteraction.Collide);

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
        _playerFootsteps = RuntimeManager.CreateInstance(_playerFootstepsRef);
    }

    private void Update()
    {
        ManageMouseRotation();
        CalculateMovementSpeed();
        ManageMovement();
        CreateGravity();
        ManageCrouching();

        if (_debugEulerAngles) Debug.Log(_player.eulerAngles);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void OnDrawGizmos()
    {
        DrawStandingHeightOnCrouch();
        DrawCharacterController();
    }

    public void SetCharacterController(bool enabled)
    {
        _characterController.enabled = enabled;
    }

    #region Public rotation and movement methods
    /// <summary>
    /// Duration means that this coroutine will take this long. 
    /// Speed means that the character will move with this speed so duration depends on the distance.
    /// Base speed is (IDK YET).
    /// </summary>
    public IEnumerator SetTransformAnimation(PlayerTargetTransform targetTransform, float duration, bool speedInsteadOfDuration = false)
    {
        StartCoroutine(MoveCharacterAnimation(targetTransform.Position, duration, speedInsteadOfDuration));
        yield return StartCoroutine(RotateCharacterAnimation(targetTransform.Rotation, duration, speedInsteadOfDuration));
    }

    /// <summary>
    /// Duration means that this coroutine will take this long. 
    /// Speed means that the character will move with this speed so duration depends on the distance.
    /// Base speed is (IDK YET).
    /// </summary>
    public IEnumerator MoveCharacterAnimation(Vector3 targetPosition, float duration, bool speedInsteadOfDuration = false)
    {
        if (_inMoveAnimation)
        {
            Debug.LogError("Already moving.");
            yield break;
        }

        _inMoveAnimation = true;

        Tween moveTween = _player.DOMove(targetPosition, duration).SetEase(Ease.InOutSine);
        if (speedInsteadOfDuration) moveTween.SetSpeedBased();

        while (moveTween.IsPlaying())
        {
            yield return null;
        }

        _inMoveAnimation = false;
    }

    /// <summary>
    /// Duration means that this coroutine will take this long. 
    /// Speed means that the character will rotate with this speed so duration depends on the distance.
    /// Base speed is (IDK YET).
    /// </summary>
    public IEnumerator RotateCharacterAnimation(Vector3 targetRotation, float duration, bool speedInsteadOfDuration = false)
    {
        if (_inRotateAnimation)
        {
            Debug.LogError("Already rotating.");
            yield break;
        }

        _inRotateAnimation = true;

        Vector3 yRotationVector = targetRotation;
        yRotationVector.x = 0;
        Vector3 xRotationVector = XRotationVectorFromEulers(targetRotation);

        Tween rotationYTween = _player.DORotate(yRotationVector, duration).SetEase(Ease.InOutSine);
        Tween rotationXTween = _playerCamera.DOLocalRotate(xRotationVector, duration).SetEase(Ease.InOutSine);

        if (speedInsteadOfDuration)
        {
            rotationYTween.SetSpeedBased();
            rotationXTween.SetSpeedBased();
        }

        yield return null;
        while (rotationYTween.IsPlaying() || rotationXTween.IsPlaying())
        {
            yield return null;
        }

        _currentXRotation = Mathf.Clamp(xRotationVector.x, -90f, 90f);
        _playerCamera.localRotation = Quaternion.Euler(_currentXRotation, 0, 0);

        _inRotateAnimation = false;
    }

    public void RotateCharacter(Vector3 targetRotation)
    {
        Vector3 yRotationVector = targetRotation;
        yRotationVector.x = 0;
        Vector3 xRotationVector = XRotationVectorFromEulers(targetRotation);

        _player.rotation = Quaternion.Euler(yRotationVector);
        _currentXRotation = Mathf.Clamp(xRotationVector.x, -90f, 90f);
        _playerCamera.localRotation = Quaternion.Euler(_currentXRotation, 0, 0);
    }
    #endregion

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
        if (!_inRotateAnimation)
        {
            //move camera up or down
            float mouseY = PlayerCameraMap.MouseY.ReadValue<float>() * _mouseSensivity * Time.deltaTime;
            _currentXRotation -= mouseY;
            _currentXRotation = Mathf.Clamp(_currentXRotation, -90, 90);
            _playerCamera.localRotation = Quaternion.Euler(_currentXRotation, 0, 0);

            //rotate whole player object left or right
            float mouseX = PlayerCameraMap.MouseX.ReadValue<float>() * _mouseSensivity * Time.deltaTime;
            transform.Rotate(Vector3.up * mouseX);
        }
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
        Vector2 inputVector = PlayerMovementMap.Movement.ReadValue<Vector2>();
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
        if (!_isCrouching && !IsOnStairs && PlayerMovementMap.Crouch.ReadValue<float>() > 0)
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
        if ((_isCrouching && PlayerMovementMap.Crouch.ReadValue<float>() == 0 && CanStandUp())
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
            if (!CanStandUp())
            {
                _isCrouching = true;
                _standUpCoroutine = null;
                yield break;
            }

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

    private bool CanStandUp()
    {
        //starts from camera but we need to start from top so +offset
        float castDistance = _cameraTopOffset;
        //radius is inside of the sphereCast so we don't need it here
        castDistance -= _characterController.radius;
        //+height that is requierd to stand up
        castDistance += _standHeight - _characterController.height;

        RaycastHit[] hits = Physics.SphereCastAll(_playerCamera.position, _characterController.radius, Vector3.up, castDistance);

        int length = 0;

        foreach (RaycastHit hit in hits)
        {
            if (!hit.collider.isTrigger) length++;
        }

        if (length > 1)
        {
            return false;
        }

        return true;
    }

    private void DrawStandingHeightOnCrouch()
    {
        if (_isCrouching)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(_playerCamera.position, Vector3.up * (_cameraTopOffset + _standHeight - _characterController.height));
        }
    }

    private void DrawCharacterController()
    {
        Gizmos.color = Color.green;
        Gizmos.matrix = transform.localToWorldMatrix;

        Vector3 center = _characterController.center;
        float height = _characterController.height;
        float radius = _characterController.radius;

        Vector3 topSphereCenter = center;
        Vector3 bottomSphereCenter = center;

        topSphereCenter.y += (height / 2) - radius;
        bottomSphereCenter.y -= (height / 2) - radius;

        Gizmos.DrawWireSphere(topSphereCenter, radius);
        Gizmos.DrawWireSphere(bottomSphereCenter, radius);

        Gizmos.DrawLine(topSphereCenter + Vector3.forward * radius, bottomSphereCenter + Vector3.forward * radius);
        Gizmos.DrawLine(topSphereCenter - Vector3.forward * radius, bottomSphereCenter - Vector3.forward * radius);
        Gizmos.DrawLine(topSphereCenter + Vector3.right * radius, bottomSphereCenter + Vector3.right * radius);
        Gizmos.DrawLine(topSphereCenter - Vector3.right * radius, bottomSphereCenter - Vector3.right * radius);
    }
}