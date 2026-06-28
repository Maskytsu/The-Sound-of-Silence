using FMODUnity;
using System.Collections;
using UnityEngine;

public class HiddenQuestText : QuestText
{
    [HideInInspector] public QuestScriptable Quest;

    protected override void Start()
    {
        TMP.enabled = false;
        StartCoroutine(ShowHideTextAnimation());
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    public override void DestroyQuestText()
    {
        StopAllCoroutines();
        base.DestroyQuestText();
    }

    private IEnumerator ShowHideTextAnimation()
    {
        while (true)
        {
            float spareTime1 = Random.Range(10f, 30f);
            float displayTime = Random.Range(1f, 4f);
            float spareTime2 = Random.Range(10f, 30f);
            int randomIndex = Random.Range(0, Quest.QuestTexts.Count);

            yield return new WaitForSeconds(spareTime1);

            RuntimeManager.PlayOneShot(FmodEvents.Instance.HiddenQuestAppeared);
            TMP.text = Quest.QuestTexts[randomIndex];
            TMP.enabled = true;

            yield return new WaitForSeconds(displayTime);
            TMP.enabled = false;

            yield return new WaitForSeconds(spareTime2);
        }
    }
}