using UnityEngine;

public class Note : Interactable
{
    [Space]
    [SerializeField] private PaperSheetDisplay _noteDisplay;

    protected override void Interact()
    {
        Instantiate(_noteDisplay);
    }
}