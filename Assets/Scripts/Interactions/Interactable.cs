using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] protected GameObject _prompt;

    public virtual void ShowPrompt()
    {
        _prompt.SetActive(true);
    }

    public virtual void HidePrompt()
    {
        _prompt.SetActive(false);
    }

    public abstract void Interact();
}
