using Cinemachine;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [field: SerializeField] public GameObject Player { get; private set; }
    [field: SerializeField] public GameObject PlayerVisuals { get; private set; }
    [field: SerializeField] public PlayerMovement PlayerMovement { get; private set; }
    [field: SerializeField] public PlayerEquipment PlayerEquipment { get; private set; }
    [field: SerializeField] public PlayerInteractor PlayerInteractor { get; private set; }
    [field: SerializeField] public CharacterController PlayerCharacterController { get; private set; }
    [field: SerializeField] public CinemachineVirtualCamera PlayerVirtualCamera { get; private set; }

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