using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] protected Canvas _promptInteract;

    public virtual void ShowPrompt()
    {
        _promptInteract.enabled = true;
    }

    public virtual void HidePrompt()
    {
        _promptInteract.enabled = false;
    }

    public abstract void Interact();
}
