using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFlashlight : Item
{
    public GameObject LightCone;

    public override void UseItem()
    {
        //turn on or off flashlight
        LightCone.SetActive(!LightCone.activeSelf);
    }
}
