using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [SerializeField] private SceneSetup _sceneSetup;

    private List<QuestScriptable> _questsOnScene;
    private List<QuestScriptable> _currentQuests;

    private void Awake()
    {
        CreateInstance();
        _questsOnScene = _sceneSetup.QuestSequence;
        _currentQuests = new List<QuestScriptable>();
    }

    private void OnDestroy()
    {
        foreach(var quest in _questsOnScene)
        {
            quest.ClearSubscribers();
        }
    }

    public void StartQuest(QuestScriptable quest)
    {
        if (!_questsOnScene.Contains(quest))
        {
            Debug.LogError("Quest not added to Quest Sequence in Scene Setup (GameManager obj)!");
            return;
        }

        if (_currentQuests.Contains(quest))
        {
            Debug.LogWarning("Quest already started!");
            return;
        }

        _currentQuests.Add(quest);
        quest.OnQuestStart?.Invoke();
        HUD.Instance.QuestDisplay.DisplayNewQuest(quest);

        Debug.Log(quest.name + " started");
        quest.OnQuestEnd += () => Debug.Log(quest.name + " ended");
    }

    public IEnumerator StartQuestDelayed(QuestScriptable quest, float delayTime = 2f)
    {
        yield return new WaitForSeconds(delayTime);
        StartQuest(quest);
    }

    public void EndQuest(QuestScriptable quest)
    {
        if (!_currentQuests.Contains(quest))
        {
            Debug.LogError("Quest was not started properly yet!");
            return;
        }

        _currentQuests.Remove(quest);
        quest.OnQuestEnd?.Invoke();
    }

    private void CreateInstance()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one QuestManager in the scene.");
        }
        Instance = this;
    }
}
