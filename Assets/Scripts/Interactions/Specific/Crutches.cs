using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crutches : Interactable
{
    protected override void Interact()
    {
        RuntimeManager.PlayOneShot(FmodEvents.Instance.GrabbingCrutches);
        gameObject.SetActive(false);
    }
}