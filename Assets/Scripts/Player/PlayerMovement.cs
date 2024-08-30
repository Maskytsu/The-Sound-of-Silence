using FMOD.Studio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("GameObjects")]
    [SerializeField] private Transform mainCamera;
    [SerializeField] private Transform groundCheck;

    private PlayerInputActions playerInputActions;
    private CharacterController characterController;
    private PlayerEquipment playerEquipment;

    [Header("Gravity Parameters")]
    [SerializeField] private float pullingVelocity = 20f;
    [SerializeField] private LayerMask groundMask;

    [Header("Movement Speed Parameters")]
    [SerializeField] private float normalWalkSpeed = 3;
    [SerializeField] private float slowWalkSpeed = 2.5f;
    [SerializeField] private float normalSneakSpeed = 1.5f;
    [SerializeField] private float slowSneakSpeed = 1.25f;

    [Header("Sensivity Parameters")]
    [SerializeField] private float mouseSensivity = 15;

    [Header("Sneaking Parameters")]
    [SerializeField] private float sneakHeight = 1.5f;
    [SerializeField] private float standHeight = 3;
    [SerializeField] private float timeToSneakStand = 0.35f;
    [SerializeField] private float cameraOffset = 0.5f;
    [SerializeField] private Vector3 sneakCenter = new Vector3(0, 0.75f, 0);
    [SerializeField] private Vector3 standCenter = new Vector3(0, 0, 0);
    
    private float xRotation = 0;
    private float speed;
    private float currentPullingVelocity;
    private bool isGrounded;
    private bool isSneaking = false;
    private bool duringSneakStandAnimation = false;

    private EventInstance playerFootsteps;


    private void Awake()
    {
        //assign proper values
        playerInputActions = new PlayerInputActions();
        characterController = GetComponent<CharacterController>();
        playerEquipment = GetComponent<PlayerEquipment>();

        //setup
        Cursor.lockState = CursorLockMode.Locked;
        playerInputActions.PlayerMap.Enable();
        speed = normalWalkSpeed;
    }

    private void Start()
    {
        playerFootsteps = AudioManager.instance.CreateEventInstance(FMODEvents.instance.playerFootsteps);
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
        float mouseY = playerInputActions.PlayerMap.MouseY.ReadValue<float>() * mouseSensivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);
        mainCamera.localRotation = Quaternion.Euler(xRotation, 0, 0);

        //move left or right
        float mouseX = playerInputActions.PlayerMap.MouseX.ReadValue<float>() * mouseSensivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);
    }

    private void ManageMovementSpeed()
    {
        bool handsAreEmpty = playerEquipment.handsAreEmpty;
        if (!isSneaking && handsAreEmpty) speed = normalWalkSpeed;
        else if (!isSneaking && !handsAreEmpty) speed = slowWalkSpeed;
        else if (isSneaking && handsAreEmpty) speed = normalSneakSpeed;
        else if (isSneaking && !handsAreEmpty) speed = slowSneakSpeed;
    }

    private void MoveCharacter()
    {
        Vector2 inputVector = playerInputActions.PlayerMap.Movement.ReadValue<Vector2>();
        Vector3 movement = transform.right * inputVector.x + transform.forward * inputVector.y;
        if(isGrounded) characterController.Move(movement * speed * Time.deltaTime);
           
        if (inputVector != Vector2.zero && isGrounded)
        {
            PLAYBACK_STATE playbackState;
            playerFootsteps.getPlaybackState(out playbackState);
            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                playerFootsteps.start();
            }
        }
        else
        {
            playerFootsteps.stop(STOP_MODE.ALLOWFADEOUT);
        }
    }

    //------------------------------------------------------------------------------------------------------gravity
    private void CreateGravity()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, characterController.radius, groundMask);

        if(isGrounded && currentPullingVelocity > 2) //there is always some velocity for smoother transitions
        {
            currentPullingVelocity = 2;
        }

        currentPullingVelocity += pullingVelocity * Time.deltaTime;
        characterController.Move(Vector3.down * currentPullingVelocity * Time.deltaTime); //it is doubly multiplied by time because of how physics equation for gravity works
    }

    //------------------------------------------------------------------------------------------------------sneaking
    private void ManageSneaking()
    {
        //check if changing state is needed
        if (!duringSneakStandAnimation &&
            ((playerInputActions.PlayerMap.Sneak.ReadValue<float>() > 0 && !isSneaking)
            || (playerInputActions.PlayerMap.Sneak.ReadValue<float>() == 0 && isSneaking)))
        {
            StartCoroutine(SneakingStanding());
        }
    }

    private IEnumerator SneakingStanding()
    {
        //check if standing up is possible
        float castDistance = standHeight - sneakHeight + cameraOffset; //offset between cameras pos and top of character controller
        castDistance = castDistance - characterController.radius;
        if (isSneaking && Physics.SphereCast(mainCamera.position, characterController.radius, Vector3.up, out RaycastHit hitInfo, castDistance))
        {
            Debug.Log(hitInfo.transform.gameObject.name);
            yield break;
        }

        duringSneakStandAnimation = true;

        float timeElapsed = 0;
        float characterTargetHeight = isSneaking ? standHeight : sneakHeight;
        float characterCurrentHeight = characterController.height;
        Vector3 characterTargetCenter = isSneaking ? standCenter : sneakCenter;
        Vector3 characterCurrentCenter = characterController.center;
        Vector3 groundCheckTargetPosition = isSneaking ? 
            new Vector3(groundCheck.localPosition.x, groundCheck.localPosition.y - (standHeight - sneakHeight), groundCheck.localPosition.z) :
            new Vector3(groundCheck.localPosition.x, groundCheck.localPosition.y + (standHeight - sneakHeight), groundCheck.localPosition.z);
        Vector3 groundCheckCurrentPosition = groundCheck.localPosition;

        //testing FMOD
        if (!isSneaking) AudioManager.instance.PlayOneShot(FMODEvents.instance.playerStartedSneaking, transform.position);

        //changing height and center of CharacterController in given time, also changing local position of ground check
        while (timeElapsed < timeToSneakStand)
        {
            characterController.height = Mathf.Lerp(characterCurrentHeight, characterTargetHeight, timeElapsed/ timeToSneakStand);
            characterController.center = Vector3.Lerp(characterCurrentCenter, characterTargetCenter, timeElapsed / timeToSneakStand);
            groundCheck.localPosition = Vector3.Lerp(groundCheckCurrentPosition, groundCheckTargetPosition, timeElapsed / timeToSneakStand);
            if (!isSneaking && isGrounded) characterController.Move(Vector3.down * pullingVelocity); //pull down when CharacterControllers height is reduced
            timeElapsed  += Time.deltaTime;
            yield return null;
        }

        characterController.height = characterTargetHeight;
        characterController.center = characterTargetCenter;
        groundCheck.localPosition = groundCheckTargetPosition;

        isSneaking = !isSneaking;
        duringSneakStandAnimation = false;
    }

    //------------------------------------------------------------------------------------------------------enable/disable map
    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerInputActions.PlayerMap.Enable();
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.Confined;
        playerInputActions.PlayerMap.Disable();
    }

    //------------------------------------------------------------------------------------------------------draw sneaking/stand up height Gizmo
    private void OnDrawGizmos()
    {
        if (isSneaking)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(mainCamera.position, Vector3.up * (standHeight - sneakHeight + cameraOffset));
        }
    }
}
