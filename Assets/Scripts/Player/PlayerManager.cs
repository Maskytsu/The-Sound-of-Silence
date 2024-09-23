using Cinemachine;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Editor;

public class PlayerManager : MonoBehaviour
{
    [field: SerializeField] public GameObject Player { get; private set; }
    [field: SerializeField] public PlayerMovement PlayerMovement { get; private set; }
    [field: SerializeField] public PlayerEquipment PlayerEquipment { get; private set; }
    [field: SerializeField] public PlayerInteractor PlayerInteractor { get; private set; }
    [field: SerializeField] public Camera Camera { get; private set; }
    [field: SerializeField] public Camera UIInteractCamera { get; private set; }
    [field: SerializeField] public CinemachineBrain CameraBrain { get; private set; }
    [field: SerializeField] public CinemachineVirtualCamera VirtualMainCamera { get; private set; }
    [field: SerializeField] public CinemachineVirtualCamera VirtualLookAtCamera { get; private set; }

    public static PlayerManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one Player in the scene.");
        }
        Instance = this;
    }
}