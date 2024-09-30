using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pills : Interactable
{
    public override void Interact()
    {
        GameState.Instance.TookPills = true;
        Destroy(transform.parent.gameObject);
    }
}
