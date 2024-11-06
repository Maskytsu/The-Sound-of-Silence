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
        if (!GameState.Instance.TookPills) SceneManager.LoadScene(_scene2);
        else SceneManager.LoadScene(_secretEnding1);
    }
}
