using NaughtyAttributes;

public class Scene1ResetHandler : SingletonMonobehaviour<Scene1ResetHandler>
{
    [ReadOnly] public bool SceneWasReseted;

    protected override void Awake()
    {
        base.Awake();

        if (DontDestroyOnLoadChecker.Instance != null)
        {
            SceneWasReseted = true;
            Destroy(DontDestroyOnLoadChecker.Instance.gameObject);
        }
        else
        {
            SceneWasReseted = false;
        }
    }
}