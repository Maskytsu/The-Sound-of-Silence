using UnityEngine;

public class Scene6ResetHandler : MonoBehaviour
{
    public bool SceneWasReseted { get; private set; }

    [Header("Prefabs")]
    [SerializeField] private SceneResetedChecker _checkerPrefab;
    [Header("Scene Objects")]
    [SerializeField] private CatchingPlayerMonsterState _catchingState;

    private void Awake()
    {
        if (SceneResetedChecker.Instance != null)
        {
            SceneWasReseted = true;
            Destroy(SceneResetedChecker.Instance.gameObject);
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
