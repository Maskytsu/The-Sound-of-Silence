using Cinemachine;
using UnityEngine;

public class PlayerObjectsHolder : MonoBehaviour
{
    public static PlayerObjectsHolder Instance { get; private set; }

    [field: SerializeField] public GameObject Player { get; private set; }
    [field: SerializeField] public PlayerMovement PlayerMovement { get; private set; }
    [field: SerializeField] public PlayerEquipment PlayerEquipment { get; private set; }
    [field: SerializeField] public PlayerInteractor PlayerInteractor { get; private set; }
    [field: SerializeField] public CharacterController PlayerCharacterController { get; private set; }
    [field: SerializeField] public CinemachineVirtualCamera PlayerVirtualCamera { get; private set; }

    private void Awake()
    {
        CreateInstance();
    }

    private void CreateInstance()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one PlayerObjectsHolder in the scene.");
        }
        Instance = this;
    }
}