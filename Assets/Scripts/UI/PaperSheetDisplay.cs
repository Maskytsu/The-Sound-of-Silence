using System;
using UnityEngine;

public class PaperSheetDisplay : MonoBehaviour
{
    public Action OnReadingEnd;

    private InputProvider InputProvider => InputProvider.Instance;

    private void Awake()
    {
        OpenPaperSheet();
    }

    private void Update()
    {
        ManageInput();
    }

    private void ManageInput()
    {
        if (InputProvider.UIMap.Cancel.WasPerformedThisFrame())
        {
            ClosePaperSheet();
        }
    }

    private void OpenPaperSheet()
    {
        Time.timeScale = 0f;

        InputProvider.SaveMapStates();
        InputProvider.TurnOffGameplayMaps();
    }

    private void ClosePaperSheet()
    {
        Time.timeScale = 1f;
        InputProvider.LoadMapStatesAndApplyThem();
        OnReadingEnd?.Invoke();

        Destroy(gameObject);
    }
}
