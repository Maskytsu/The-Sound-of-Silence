using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] private GameObject prompt;
    public void ShowPrompt()
    {
        prompt.SetActive(true);
    }

    public void HidePrompt()
    {
        prompt.SetActive(false);
    }

    public abstract void Interact();
}
