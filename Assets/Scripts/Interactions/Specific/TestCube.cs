using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TESTCube : Interactable
{
    public override void Interact()
    {
        Destroy(transform.parent.gameObject);
    }
}
