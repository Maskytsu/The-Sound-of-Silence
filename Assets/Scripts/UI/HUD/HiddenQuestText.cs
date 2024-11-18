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
        _tmp.enabled = false;
        StartCoroutine(ShowHideTextAnimation());
    }

    private IEnumerator ShowHideTextAnimation()
    {
        while (true)
        {
            float spareTime1 = Random.Range(7.5f, 10f);
            float displayTime = Random.Range(1f, 4f);
            float spareTime2 = Random.Range(7.5f, 10f);
            int randomIndex = Random.Range(0, Quest.QuestTexts.Count);

            yield return new WaitForSeconds(spareTime1);

            _tmp.text = Quest.QuestTexts[randomIndex];
            _tmp.enabled = true;

            yield return new WaitForSeconds(displayTime);
            _tmp.enabled = false;

            yield return new WaitForSeconds(spareTime2);
        }
    }
}