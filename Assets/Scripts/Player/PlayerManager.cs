using Cinemachine;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Editor;

public class PlayerManager : MonoBehaviour
{
    [field: SerializeField] public GameObject Player { get; private set; }
    [field: SerializeField] public GameObject PlayerVisuals { get; private set; }
    [field: SerializeField] public PlayerMovementAndRotation PlayerMovementAndRotation { get; private set; }
    [field: SerializeField] public PlayerEquipment PlayerEquipment { get; private set; }
    [field: SerializeField] public PlayerInteractor PlayerInteractor { get; private set; }
    [field: SerializeField] public CharacterController PlayerCharacterController { get; private set; }
    [field: SerializeField] public Camera MainCamera { get; private set; }
    [field: SerializeField] public Camera PhoneInteractCamera { get; private set; }
    [field: SerializeField] public CinemachineBrain CameraBrain { get; private set; }
    [field: SerializeField] public CinemachineVirtualCamera VirtualMainCamera { get; private set; }
    [field: SerializeField] public CinemachineVirtualCamera VirtualLookAtCamera { get; private set; }

    public static PlayerManager Instance { get; private set; }

    private void Awake()
    {
        CreateInstance();
    }

    private void CreateInstance()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one Player in the scene.");
        }
        Instance = this;
    }
}