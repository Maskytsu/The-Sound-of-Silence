using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crutches : Interactable
{
    protected override void Interact()
    {
        //could make animation in which one disapears and then other
        //something like picking up suit in outer wilds
        Destroy(gameObject);
    }
}
