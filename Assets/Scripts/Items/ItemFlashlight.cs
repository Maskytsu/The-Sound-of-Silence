using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class ItemFlashlight : Item
{
    [SerializeField] private GameObject lightCone;
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
        lightCone.SetActive(!lightCone.activeSelf);
    }
}
