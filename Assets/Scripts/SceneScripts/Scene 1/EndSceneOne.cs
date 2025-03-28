using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSceneOne : MonoBehaviour
{
    [Header("Scene Objects")]
    [SerializeField] private GoSleepQuestHandler goSleepQuestHandler;
    [Header("Parameters")]
    [Scene, SerializeField] private string _secretEnding1;
    [Scene, SerializeField] private string _scene2;

    private void Start()
    {
        goSleepQuestHandler.OnAnimationEnd += ChangeScene;
    }

    private void ChangeScene()
    {
        if (!GameState.Instance.TookPills) GameManager.Instance.LoadSceneAndSaveGameState(_scene2);
        else GameManager.Instance.LoadSceneAndSaveGameState(_secretEnding1);
    }
}