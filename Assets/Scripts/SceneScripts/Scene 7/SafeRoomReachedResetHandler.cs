using NaughtyAttributes;
using System;
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
    [SerializeField] private List<GameObject> _objToTurnOffIfGun;

    private bool _spawnInSafeRoom = false;
    private bool _tookGun = false;

    private ItemManager ItemManager => ItemManager.Instance;

    //needs to work on Awake from Scene7ResetHandler
    public void PrepareScene(Scene7ResetHandler resetHandler)
    {
        SetActiveObjects(_objToTurnOn, true);
        SetActiveObjects(_objToTurnOff, false);

        if (resetHandler.TookGun)
        {
            _tookGun = true;
            SetActiveObjects(_objToTurnOffIfGun, false);
        }

        _uiManager.OverrideDisplayHour(false);

        TeleportMonster();

        _spawnInSafeRoom = true;
    }

    private void Start()
    {
        if (_spawnInSafeRoom)
        {
            //it is like that because of starts order
            //sometimes dictionaries are created before this and sometimes are created after
            try
            {
                Debug.Log("Added items without errors");

                ItemManager.ItemsPerType[ItemType.KEYS].PlayerHasIt = true;
                if (_tookGun) ItemManager.ItemsPerType[ItemType.GUN].PlayerHasIt = true;
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);

                ItemManager.OnDictionariesCreated += () =>
                    ItemManager.ItemsPerType[ItemType.KEYS].PlayerHasIt = true;

                if (_tookGun)
                {
                    ItemManager.OnDictionariesCreated += () =>
                        ItemManager.ItemsPerType[ItemType.GUN].PlayerHasIt = true;
                }
            }

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