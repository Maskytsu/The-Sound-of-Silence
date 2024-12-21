using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : Interactable
{
    protected override void Interact()
    {
        gameObject.SetActive(false);
    }
}
