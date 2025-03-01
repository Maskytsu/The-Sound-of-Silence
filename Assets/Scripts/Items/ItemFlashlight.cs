using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFlashlight : Item
{
    public GameObject LightCone;

    public override void UseItem()
    {
        //turn on or off flashlight
        if (LightCone.activeSelf)
        {
            RuntimeManager.PlayOneShot(FmodEvents.Instance.FlashlightClickOff);
            LightCone.SetActive(false);
        }
        else
        {
            RuntimeManager.PlayOneShot(FmodEvents.Instance.FlashlightClickOn);
            LightCone.SetActive(true);
        }
    }
}