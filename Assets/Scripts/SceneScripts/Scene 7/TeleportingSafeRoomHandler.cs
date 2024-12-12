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

    private Color _savedGrassHealthyColor;
    private Color _savedGrassDryColor;

    private void Start()
    {
        _savedGrassHealthyColor = _terrain.terrainData.detailPrototypes[0].healthyColor;
        _savedGrassDryColor = _terrain.terrainData.detailPrototypes[0].dryColor;

        _playerCloseTrigger.OnObjectTriggerEnter += SetCloseView;
        _playerCloseTrigger.OnObjectTriggerExit += SetFarView;
    }

    private void SetCloseView()
    {
        TurnOnObjects(_closeObjects);
        TurnOffObjects(_farObjects);

        //doesn't work - i want to make grass transparent for that time
        DetailPrototype detailPrototype = _terrain.terrainData.detailPrototypes[0];
        detailPrototype.healthyColor = new Color(0, 0, 0, 0);
        detailPrototype.dryColor = new Color(0, 0, 0, 0);
        _terrain.terrainData.detailPrototypes[0] = detailPrototype;
    }

    private void SetFarView()
    {
        //this is basic view + portal
        TurnOnObjects(_farObjects);
        TurnOffObjects(_closeObjects);

        //doesn't work - i want to make grass transparent for that time
        DetailPrototype detailPrototype = _terrain.terrainData.detailPrototypes[0];
        detailPrototype.healthyColor = _savedGrassHealthyColor;
        detailPrototype.dryColor = _savedGrassDryColor;
        _terrain.terrainData.detailPrototypes[0] = detailPrototype;
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
