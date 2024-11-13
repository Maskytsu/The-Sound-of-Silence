using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFlashlight : Item
{
    public GameObject LightCone;
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
        LightCone.SetActive(!LightCone.activeSelf);
    }
}
