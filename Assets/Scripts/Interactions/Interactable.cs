using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] protected GameObject _promptInteract;

    public virtual void ShowPrompt()
    {
        _promptInteract.SetActive(true);
    }

    public virtual void HidePrompt()
    {
        _promptInteract.SetActive(false);
    }

    public abstract void Interact();
}
