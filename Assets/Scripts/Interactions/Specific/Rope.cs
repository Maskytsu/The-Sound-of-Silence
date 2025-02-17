using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : Interactable
{
    protected override void Interact()
    {
        RuntimeManager.PlayOneShot(FmodEvents.Instance.GrabbingRope);
        gameObject.SetActive(false);
    }
}