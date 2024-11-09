using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pills : Interactable
{
    protected override void Interact()
    {
        GameState.Instance.TookPills = true;
        Destroy(gameObject);
    }
}
