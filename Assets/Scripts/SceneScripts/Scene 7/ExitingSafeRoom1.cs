using UnityEngine;

public class ExitingSafeRoom1 : Checkpoint
{
    [Header("Exit Door")]
    //from the safe room 1 that is first in the house (safe room not reached)
    [SerializeField] private Door _teleportingExitDoor;
    [SerializeField] private Trigger _closeExitDoorTrigger;
    [SerializeField] private GameObject _exitDoorBlockade;
    [Space]
    [SerializeField] private Trigger _tpMonsterTrigger;
    [SerializeField] private TeleportingSafeRoomHandler _tpRoomHandler;


    private void Start()
    {
        _closeExitDoorTrigger.OnObjectTriggerEnter += CloseDoor;
        _closeExitDoorTrigger.OnObjectTriggerEnter += () => _tpMonsterTrigger.gameObject.SetActive(false);
    }

    public override void ResetToThisCheckpoint()
    {
        if (_checkpointPosition == null)
        {
            Debug.LogWarning("Reseted to checkpoint that has no position setuped!");
            return;
        }

        Debug.Log("Reseted to this checkpoint: " + gameObject.name);
        _tpMonsterTrigger.gameObject.SetActive(true);
        PlayerObjects.Instance.PlayerMovement.SetTransformInstant(_checkpointPosition, true);
        _tpRoomHandler.TeleportMonster();
        ResetExitDoor();
    }

    private void ResetExitDoor()
    {
        _exitDoorBlockade.SetActive(false);
        _closeExitDoorTrigger.gameObject.SetActive(true);
        _teleportingExitDoor.InteractionHitbox.gameObject.SetActive(true);
        _teleportingExitDoor.SetOpened(true);
    }

    private void CloseDoor()
    {
        _exitDoorBlockade.SetActive(true);
        _closeExitDoorTrigger.gameObject.SetActive(false);

        _teleportingExitDoor.InteractionHitbox.gameObject.SetActive(false);

        //this is potentially bugged - if door in switch animation (closed -> opened) it wont work
        //closeDoorTrigger must be placed in the correct position
        //closing door must push player into it before end of animation - befor the could open it
        if (_teleportingExitDoor.IsOpened) _teleportingExitDoor.SwitchDoorAnimated();
    }
}
