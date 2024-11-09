using System;
using UnityEngine;

public class GlassOfWater : Interactable
{
    protected override void Interact()
    {
        //could make animation in which water disapears from glass instead of whole glass
        Destroy(gameObject);
    }
}
