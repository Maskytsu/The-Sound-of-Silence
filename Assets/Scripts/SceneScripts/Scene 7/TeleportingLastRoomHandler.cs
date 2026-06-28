using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportingLastRoomHandler : MonoBehaviour
{
    [Header("TP Activation")]
    [SerializeField] private Trigger _lastSafeRoomCloseExitDoorTrigger;
    [SerializeField] private Door _lastSafeRoomExitDoor;
    [SerializeField] private Transform _lastRoom;
    [SerializeField] private Transform _lastRoomTargetPos;

    [Header("Visuals To Look Right After TP")]
    [SerializeField] private Terrain _terrain;
    [SerializeField] private List<GameObject> _objectsInTheWay;

    [Header("Open Outside Door")]
    [SerializeField] private Trigger _openOutsideDoorTrigger;
    [SerializeField] private Door _outsideRoomDoor;

    [Header("TP Monster There")]
    [SerializeField] private Trigger _tpMonsterThereTrigger;
    [SerializeField] private Transform _monsterTpPos;

    [Header("Close Outside Door")]
    [SerializeField] private Trigger _closeOutsideDoorTrigger;
    [SerializeField] private GameObject _outsideDoorBlockade;
    [SerializeField] private MeshRenderer _stairsEmissiveRenderer;
    [SerializeField] private Material _stairsBaseMaterial;
    [SerializeField] private StormEffect _storm;
    [SerializeField] private KillMonsterQuestHandler _killQuestManager;

    private float _savedDetailDistance;
    private bool _shouldCheckLastSafeRoomDoor = false;
    private bool _shouldCheckGrassInView = false;
    private bool _shouldCheckOutsideDoor = false;
    private Vector3 _lastRoomBasePos;

    private void Start()
    {
        _lastRoomBasePos = _lastRoom.transform.position;
        _lastSafeRoomCloseExitDoorTrigger.OnObjectTriggerEnter += () => _shouldCheckLastSafeRoomDoor = true;

        _openOutsideDoorTrigger.OnObjectTriggerEnter += OpenOutsideDoor;
        _tpMonsterThereTrigger.OnObjectTriggerEnter += TpMonsterThere;
        _closeOutsideDoorTrigger.OnObjectTriggerEnter += CloseOutsideDoor;
     }

    private void Update()
    {
        if (_shouldCheckLastSafeRoomDoor) CheckLastSafeRoomDoor();
        if (_shouldCheckGrassInView) CheckGrassInView();
        if (_shouldCheckOutsideDoor) CheckOutsideDoor();
    }

    private void CheckLastSafeRoomDoor()
    {
        if (!_lastSafeRoomExitDoor.IsOpened)
        {
            _lastSafeRoomCloseExitDoorTrigger.gameObject.SetActive(false);
            _shouldCheckLastSafeRoomDoor = false;
            StartCoroutine(TeleportRoomAndPlayer());
        }
    }

    private void CheckOutsideDoor()
    {
        if (!_outsideRoomDoor.IsOpened)
        {
            _shouldCheckOutsideDoor = false;

            TurnOnObjectsAndTurnOffRoom();
        }
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

    private void ResetRoom()
    {
        var monsterSM = MonsterStateMachine.Instance;
        monsterSM.GetMonsterState<CatchingPlayerMonsterState>().OnPlayerCatched -= ResetRoom;

        _openOutsideDoorTrigger.gameObject.SetActive(true);
        _tpMonsterThereTrigger.gameObject.SetActive(true);
        _closeOutsideDoorTrigger.gameObject.SetActive(true);

        _outsideDoorBlockade.gameObject.SetActive(false);
        _lastRoom.position = _lastRoomBasePos;

        _outsideRoomDoor.SetOpened(false);
        _storm.gameObject.SetActive(false);
    }

    private void TurnOnObjectsAndTurnOffRoom()
    {
        SetActiveObjects(_objectsInTheWay, true);
        _lastRoom.gameObject.SetActive(false);
        _terrain.detailObjectDistance = _savedDetailDistance;
        _storm.gameObject.SetActive(true);
    }

    private void OpenOutsideDoor()
    {
        _openOutsideDoorTrigger.gameObject.SetActive(false);

        if (_outsideRoomDoor.IsOpened) 
        {
            Debug.LogWarning("Door is already open for some reason!");
            return;
        }

        _outsideRoomDoor.SwitchDoorAnimated();
    }

    private void TpMonsterThere()
    {
        _tpMonsterThereTrigger.gameObject.SetActive(false);
        _shouldCheckGrassInView = true;

        var monsterSM = MonsterStateMachine.Instance;
        if (monsterSM == null)
        {
            Debug.LogWarning("Monster is null. Was it killed?");
            return;
        }

        var tpChosenState = monsterSM.GetMonsterState<TeleportingChosenMonsterState>();
        tpChosenState.SetUpDestination(_monsterTpPos.position, true);
        monsterSM.ChangeState(tpChosenState);

        tpChosenState.OnTpDestinationReached += StartChasingPlayer;

        monsterSM.GetMonsterState<CatchingPlayerMonsterState>().OnPlayerCatched += ResetRoom;
    }

    private void StartChasingPlayer()
    {
        var monsterSM = MonsterStateMachine.Instance;
        monsterSM.ChangeState<LookingForPlayerMonsterState>();
        AudioManager.Instance.PlayOneShotOccludedRI(FmodEvents.Instance.OCC_MonsterAngry, monsterSM.MonsterTransform);
    }

    private void CloseOutsideDoor()
    {
        _closeOutsideDoorTrigger.gameObject.SetActive(false);

        var monsterSM = MonsterStateMachine.Instance;
        if (monsterSM == null) Debug.LogWarning("Monster is null. Was it killed?");
        else
        {
            monsterSM.ChangeState<PerishingMonsterState>();
            if (!_killQuestManager.QuestEnded) _killQuestManager.FailQuest();
        }

        _outsideDoorBlockade.SetActive(true);

        if (!_outsideRoomDoor.IsOpened)
        {
            Debug.LogWarning("Door is already closed for some reason!");
        }
        else
        {
            _outsideRoomDoor.SwitchDoorAnimated();
        }

        _stairsEmissiveRenderer.SetMaterials(new List<Material>() { _stairsBaseMaterial, _stairsBaseMaterial });

        _shouldCheckGrassInView = false;
        _terrain.detailObjectDistance = _savedDetailDistance;

        _shouldCheckOutsideDoor = true;
    }

    private IEnumerator TeleportRoomAndPlayer()
    {
        SetActiveObjects(_objectsInTheWay, false);

        _savedDetailDistance = _terrain.detailObjectDistance;
        _terrain.detailObjectDistance = 0;

        //tp player and room
        CharacterController playerCharacterController = PlayerObjects.Instance.Player.GetComponent<CharacterController>();
        playerCharacterController.enabled = false;
        PlayerObjects.Instance.Player.transform.parent = _lastRoom;
        _lastRoom.position = _lastRoomTargetPos.position;
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
}