using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] private GameObject _prompt;
    public void ShowPrompt()
    {
        _prompt.SetActive(true);
    }

    public void HidePrompt()
    {
        _prompt.SetActive(false);
    }

    public abstract void Interact();
}
