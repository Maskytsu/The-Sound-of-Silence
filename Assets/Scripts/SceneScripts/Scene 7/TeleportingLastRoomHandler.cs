using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportingLastRoomHandler : MonoBehaviour
{
    [Header("TP Activation")]
    [SerializeField] private TeleportingLastRoom _lastRoomGood;
    [SerializeField] private TeleportingLastRoom _lastRoomBad;
    [SerializeField] private Transform _lastRoomTargetPos;

    [Header("Visuals To Look Right After TP")]
    [SerializeField] private Terrain _terrain;
    [SerializeField] private List<GameObject> _objectsInTheWay;

    [Header("Close Outside Door")]
    [SerializeField] private MeshRenderer _stairsEmissiveRenderer;
    [SerializeField] private Material _stairsBaseMaterial;
    [SerializeField] private StormEffect _storm;
    [SerializeField] private KillMonsterQuestHandler _killQuestManager;

    private float _savedDetailDistance;
    private bool _shouldCheckGrassInView = false;

    private void Start()
    {
        InitializeLastRoomEvents(_lastRoomGood);
        InitializeLastRoomEvents(_lastRoomBad);

        void InitializeLastRoomEvents(TeleportingLastRoom room)
        {
            room.OnTeleportPlayer += (TeleportingLastRoom givenRoom) => StartCoroutine(TeleportRoomAndPlayer(givenRoom));
            room.OnOutsideDoorOpened += TurnOnObjectsAndTurnOffRoom;
            room.OnTpMonsterToRoom += () => _shouldCheckGrassInView = true;
            room.OnCloseOutsideDoor += OnCloseOutsideDoor;
            room.OnRoomReset += () => _storm.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (_shouldCheckGrassInView) CheckGrassInView();
    }

    private void CheckGrassInView()
    {
        Transform playerTransform = PlayerObjects.Instance.Player.transform;

        if (playerTransform.eulerAngles.y < 90 || playerTransform.eulerAngles.y > 270 || playerTransform.position.z > 30)
        {
            _terrain.detailObjectDistance = 0;
        }
        else
        {
            _terrain.detailObjectDistance = _savedDetailDistance;
        }
    }

    private void TurnOnObjectsAndTurnOffRoom(TeleportingLastRoom room)
    {
        SetActiveObjects(_objectsInTheWay, true);
        room.gameObject.SetActive(false);
        _terrain.detailObjectDistance = _savedDetailDistance;
        _storm.gameObject.SetActive(true);
    }

    private IEnumerator TeleportRoomAndPlayer(TeleportingLastRoom room)
    {
        SetActiveObjects(_objectsInTheWay, false);

        _savedDetailDistance = _terrain.detailObjectDistance;
        _terrain.detailObjectDistance = 0;

        //tp player and room
        CharacterController playerCharacterController = PlayerObjects.Instance.Player.GetComponent<CharacterController>();
        playerCharacterController.enabled = false;
        PlayerObjects.Instance.Player.transform.parent = room.transform;
        room.transform.position = _lastRoomTargetPos.position;
        PlayerObjects.Instance.Player.transform.parent = null;
        yield return null;
        //turn off character controller for one frame because it breaks teleportation if wants to move
        playerCharacterController.enabled = true;
    }

    private void SetActiveObjects(List<GameObject> gObjects, bool activeStateToSet)
    {
        foreach (GameObject gObject in gObjects)
        {
            gObject.SetActive(activeStateToSet);
        }
    }

    private void OnCloseOutsideDoor()
    {
        var monsterSM = MonsterStateMachine.Instance;
        if (monsterSM == null) Debug.LogWarning("Monster is null. Was it killed?");
        else
        {
            monsterSM.ChangeState<PerishingMonsterState>();
            if (!_killQuestManager.QuestEnded) _killQuestManager.FailQuest();
        }

        _stairsEmissiveRenderer.SetMaterials(new List<Material>() { _stairsBaseMaterial, _stairsBaseMaterial });

        _shouldCheckGrassInView = false;
        _terrain.detailObjectDistance = _savedDetailDistance;
    }
}