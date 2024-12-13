using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportingSafeRoomHandler : MonoBehaviour
{
    [Header("Scene Objects")]
    [SerializeField] private Terrain _terrain;
    [SerializeField] private Trigger _playerCloseTrigger;
    [SerializeField] private List<GameObject> _closeObjects;
    [SerializeField] private List<GameObject> _farObjects;

    private float _savedDetailDistance;

    private void Start()
    {
        _savedDetailDistance = _terrain.detailObjectDistance;

        _playerCloseTrigger.OnObjectTriggerEnter += SetCloseView;
        _playerCloseTrigger.OnObjectTriggerExit += SetFarView;
    }

    private void SetCloseView()
    {
        TurnOnObjects(_closeObjects);
        TurnOffObjects(_farObjects);

        _terrain.detailObjectDistance = 0;
    }

    private void SetFarView()
    {
        //this is basic view + portal
        TurnOnObjects(_farObjects);
        TurnOffObjects(_closeObjects);

        _terrain.detailObjectDistance = _savedDetailDistance;
    }

    private void TurnOnObjects(List<GameObject> gObjects)
    {
        foreach (GameObject gObject in gObjects)
        {
            gObject.SetActive(true);
        }
    }

    private void TurnOffObjects(List<GameObject> gObjects)
    {
        foreach (GameObject gObject in gObjects)
        {
            gObject.SetActive(false);
        }
    }
}
