using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCube : Interactable
{
    public override void Interact()
    {
        Destroy(transform.parent.gameObject);
    }
}
