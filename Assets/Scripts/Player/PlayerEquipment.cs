using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    public bool handsAreEmpty { get; private set; }

    private PlayerInputActions playerInputActions;

    public void Awake()
    {
        playerInputActions = new PlayerInputActions();
    }
    public void Update()
    {
        ManageInputs();
    }

    private void ManageInputs()
    {
        if (playerInputActions.PlayerMap.GrabItem1.WasPerformedThisFrame())
        {

        }
        else if(playerInputActions.PlayerMap.GrabItem2.WasPerformedThisFrame())
        {

        }
        else if (playerInputActions.PlayerMap.GrabItem3.WasPerformedThisFrame())
        {

        }
        else if (playerInputActions.PlayerMap.GrabItem4.WasPerformedThisFrame())
        {

        }
        else if (playerInputActions.PlayerMap.GrabItem5.WasPerformedThisFrame())
        {

        }


        if (playerInputActions.PlayerMap.UseItem.WasPerformedThisFrame())
        {

        }
    }

    private void OpenPhone()
    {

    }

    private void TurnOnOffFlashlight()
    {

    }

    private void OpenDoors()
    {

    } 

    private void Shoot()
    {

    }

    private void OnEnable()
    {
        playerInputActions.PlayerMap.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.PlayerMap.Disable();
    }
}
