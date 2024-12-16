using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeRoomReachedResetHandler : MonoBehaviour
{
    [Header("If safe room reached")]
    [SerializeField] private TeleportingSafeRoomHandler _tpSafeRoomHandler;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private PlayerTargetTransform _SafeRoomBedPTT;
    [SerializeField] private List<GameObject> _objToTurnOn;
    [SerializeField] private List<GameObject> _objToTurnOff;
    [HorizontalLine]
    [Header("If took gun")]
    [SerializeField] private List<GameObject> _objToTurnOnIfGun;
    [SerializeField] private List<GameObject> _objToTurnOffIfGun;

    private bool _tpPlayer = false;

    //needs to work on Awake from Scene7ResetHandler
    public void PrepareScene(Scene7ResetHandler resetHandler)
    {
        SetActiveObjects(_objToTurnOn, true);
        SetActiveObjects(_objToTurnOff, false);

        if (resetHandler.TookGun)
        {
            SetActiveObjects(_objToTurnOnIfGun, true);
            SetActiveObjects(_objToTurnOffIfGun, false);
        }

        _uiManager.OverrideDisplayHour(false);

        TeleportMonster();

        _tpPlayer = true;
    }

    private void Start()
    {
        if (_tpPlayer)
        {
            StartCoroutine(PlayerObjects.Instance.PlayerMovement.SetTransformAnimation(_SafeRoomBedPTT, 0));
        }
    }

    private void TeleportMonster()
    {
        if (_tpSafeRoomHandler.MonsterSM == null)
        {
            Debug.LogWarning("Monster is null. Was it killed?");
            return;
        }


        _tpSafeRoomHandler.MonsterSM.MonsterTransform.gameObject.SetActive(true);

        _tpSafeRoomHandler.MonsterSM.ChangePatrolingPoints(_tpSafeRoomHandler.NewPatrolingPoints);
        _tpSafeRoomHandler.TpChosenState.SetUpDestination(_tpSafeRoomHandler.TpDirectionIndex);
        _tpSafeRoomHandler.MonsterSM.ChangeState(_tpSafeRoomHandler.TpChosenState);
    }

    private void SetActiveObjects(List<GameObject> gObjects, bool activeStateToSet)
    {
        foreach (GameObject gObject in gObjects)
        {
            gObject.SetActive(activeStateToSet);
        }
    }
}