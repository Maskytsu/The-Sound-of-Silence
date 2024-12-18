using System;
using UnityEngine;

public class Breakers : Interactable
{
    protected override void Interact()
    {
        //click sound here because it can be interacted after taking hearing aid

        _interactionHitbox.gameObject.SetActive(false);
        GameManager.Instance.ChangeElectricityState(true);
    }
}