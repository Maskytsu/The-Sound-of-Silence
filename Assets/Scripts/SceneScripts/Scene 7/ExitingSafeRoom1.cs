using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitingSafeRoom1 : MonoBehaviour
{
    [Header("Exit Door")]
    //from the safe room 1 that is first in the house (safe room not reached)
    [SerializeField] private Door _teleportingExitDoor;
    //from the safe room 1 that starts at the hospital (safe room reached - waking up there)
    [SerializeField] private Door _resetedExitDoor;
    [SerializeField] private Trigger _closeExitDoorTrigger;
    [SerializeField] private GameObject _exitDoorBlockade;
    [Space]
    [SerializeField] private Trigger _tpMonsterTrigger;

    private void Start()
    {
        _closeExitDoorTrigger.OnObjectTriggerEnter += CloseDoor;

        _closeExitDoorTrigger.OnObjectTriggerEnter += () => _tpMonsterTrigger.gameObject.SetActive(false);
    }

    private void CloseDoor()
    {
        _exitDoorBlockade.SetActive(true);
        _closeExitDoorTrigger.gameObject.SetActive(false);

        _teleportingExitDoor.InteractionHitbox.gameObject.SetActive(false);
        _resetedExitDoor.InteractionHitbox.gameObject.SetActive(false);

        //this is potentially bugged - if door in switch animation (closed -> opened) it wont work
        //closeDoorTrigger must be placed in the correct position
        //closing door must push player into it before end of animation - befor the could open it
        if (_teleportingExitDoor.IsOpened) _teleportingExitDoor.SwitchDoorAnimated();
        if (_resetedExitDoor.IsOpened) _resetedExitDoor.SwitchDoorAnimated();
    }
}
