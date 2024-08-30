using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFlashlight : Item
{
    [SerializeField] private GameObject _lightCone;
    /*
    private static bool activationState = false;
    private void Awake()
    {
        lightCone.SetActive(activationState);
    }
    */
    public override void UseItem()
    {
        //turn on or off flashlight
        //activationState = !activationState;
        //lightCone.SetActive(activationState);
        _lightCone.SetActive(!_lightCone.activeSelf);
    }
}
