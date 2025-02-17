using FMODUnity;
using System;
using UnityEngine;

public class GlassOfWater : Interactable
{
    protected override void Interact()
    {
        RuntimeManager.PlayOneShot(FmodEvents.Instance.DrinkingWater);
        Destroy(gameObject);
    }
}