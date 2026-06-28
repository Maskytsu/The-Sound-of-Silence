using Cinemachine;
using UnityEngine;

public class PlayerObjects : SingletonMonobehaviour<PlayerObjects>
{
    [field: SerializeField] public GameObject Player { get; private set; }
    [field: SerializeField] public PlayerMovement PlayerMovement { get; private set; }
    [field: SerializeField] public PlayerEquipment PlayerEquipment { get; private set; }
    [field: SerializeField] public PlayerInteractor PlayerInteractor { get; private set; }
    [field: SerializeField] public CinemachineVirtualCamera PlayerVirtualCamera { get; private set; }
}