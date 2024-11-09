using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pills : Interactable
{
    private void Start()
    {
        if (GameState.Instance.TookPills)
        {
            Destroy(gameObject);
        }
    }

    protected override void Interact()
    {
        GameState.Instance.TookPills = true;
        Destroy(gameObject);
    }
}
