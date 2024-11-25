using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscapeEndingDialogueChoice : MonoBehaviour
{
    public event Action OnChoiceMade;
    [HideInInspector] public EscapeEndingQuestHandler QuestHandler;

    [SerializeField] private Button _optionChris;
    [SerializeField] private Button _optionClaire;
    [SerializeField] private Button _optionPolice;
    [Space]
    [SerializeField, Scene] private string _escapeChrisEndingScene;
    [SerializeField, Scene] private string _escapeClaireEndingScene;
    [SerializeField, Scene] private string _escapePoliceEndingScene;

    private void Start()
    {
        _optionChris.onClick.AddListener(() => Choose(_escapeChrisEndingScene));
        _optionClaire.onClick.AddListener(() => Choose(_escapeClaireEndingScene));
        _optionPolice.onClick.AddListener(() => Choose(_escapePoliceEndingScene));
    }

    private void Choose(string _nextScene)
    {
        OnChoiceMade?.Invoke();
        QuestHandler.NextScene = _nextScene;
        Destroy(gameObject);
    }
}
