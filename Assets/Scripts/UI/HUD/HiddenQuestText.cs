using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HiddenQuestText : MonoBehaviour
{
    public QuestScriptable Quest;

    [SerializeField] private TextMeshProUGUI _tmp;

    private void Start()
    {
        _tmp.text = Quest.QuestTexts[0];
    }
}
