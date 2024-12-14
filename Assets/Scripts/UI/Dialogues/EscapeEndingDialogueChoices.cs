using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscapeEndingDialogueChoices : MonoBehaviour
{
    public event Action<EndingChoice> OnChoiceMade;
    [HideInInspector] public EscapeEndingQuestHandler QuestHandler;

    [SerializeField] private Button _optionChris;
    [SerializeField] private Button _optionClaire;
    [SerializeField] private Button _optionPolice;

    private void Start()
    {
        InputProvider.Instance.UnlockCursor();

        _optionChris.onClick.AddListener(() => Choose(EndingChoice.Chris));
        _optionClaire.onClick.AddListener(() => Choose(EndingChoice.Claire));
        _optionPolice.onClick.AddListener(() => Choose(EndingChoice.Police));
    }

    private void Choose(EndingChoice choice)
    {
        InputProvider.Instance.LockCursor();
        OnChoiceMade?.Invoke(choice);
        Destroy(gameObject);
    }
}
