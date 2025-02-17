using FMODUnity;
using System;
using UnityEngine;

public class Breakers : Interactable
{
    protected override void Interact()
    {
        //sound here because it can be interacted after taking hearing aid
        RuntimeManager.PlayOneShot(FmodEvents.Instance.ResetingBreakers);
        _interactionHitbox.gameObject.SetActive(false);
        GameManager.Instance.ChangeElectricityState(true);
    }
}