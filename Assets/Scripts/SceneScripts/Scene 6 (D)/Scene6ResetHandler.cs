using UnityEngine;

public class Scene6ResetHandler : MonoBehaviour
{
    public bool SceneWasReseted { get; private set; }

    [Header("Prefabs")]
    [SerializeField] private Scene6ResetedChecker _checkerPrefab;
    [Header("Scene Objects")]
    [SerializeField] private CatchingPlayerMonsterState _catchingState;

    private void Awake()
    {
        if (Scene6ResetedChecker.Instance != null)
        {
            SceneWasReseted = true;
            Destroy(Scene6ResetedChecker.Instance.gameObject);
        }
    }

    private void Start()
    {
        _catchingState.OnPlayerCatched += SpawnChecker;
    }

    private void SpawnChecker()
    {
        Instantiate(_checkerPrefab);
    }
}
