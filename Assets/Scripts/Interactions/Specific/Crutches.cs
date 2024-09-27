using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crutches : Interactable
{
    public Action OnInteract;

    public override void Interact()
    {
        OnInteract?.Invoke();
        //could make animation in which one disapears and then other
        //something like picking up suit in outer wilds
        Destroy(transform.parent.gameObject);
    }
}
