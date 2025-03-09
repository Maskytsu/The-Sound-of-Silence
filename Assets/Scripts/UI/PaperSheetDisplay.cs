using FMODUnity;
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
        RuntimeManager.PlayOneShot(FmodEvents.Instance.OpenPaperSheet);
        AudioManager.Instance.PauseGameplaySounds(true, false);

        Time.timeScale = 0f;
        InputProvider.SaveMapStates();
        InputProvider.TurnOffGameplayMaps();
    }

    private void ClosePaperSheet()
    {
        RuntimeManager.PlayOneShot(FmodEvents.Instance.ClosePaperSheet);
        AudioManager.Instance.UnpauseGameplaySounds(true, false);

        Time.timeScale = 1f;
        InputProvider.LoadMapStatesAndApplyThem();
        OnReadingEnd?.Invoke();

        Destroy(gameObject);
    }
}
