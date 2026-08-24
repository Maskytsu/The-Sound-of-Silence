using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene7ResetHandler : MonoBehaviour
{
    private const string CATCHED_BY_MONSTER_FLAG = "catched";

    [Header("Prefabs")]
    [SerializeField] private DontDestroyOnLoadChecker _checkerPrefab;
    [Header("Scene Objects")]
    [SerializeField] private CatchingPlayerMonsterState _catchingState;
    [Space]
    [ReadOnly] public bool SceneWasReseted;
    [ReadOnly] public bool SceneWasResetedByCatch;


    private void Awake()
    {
        var checker = DontDestroyOnLoadChecker.Instance;
        if (checker != null)
        {
            SceneWasReseted = true;
            SceneWasResetedByCatch = checker.CheckFlag(CATCHED_BY_MONSTER_FLAG);
            Destroy(checker.gameObject);
        }
        else
        {
            SceneWasReseted = false;
            SceneWasResetedByCatch = false;
        }
    }

    private void Start()
    {
        _catchingState.OnPlayerCatched += SpawnCheckerByCatch;
    }

    private void SpawnCheckerByCatch()
    {
        DontDestroyOnLoadChecker checker = Instantiate(_checkerPrefab);
        checker.AddFlag(CATCHED_BY_MONSTER_FLAG);
    }

    [Button]
    private void ResetSceneForTesting()
    {
        SpawnCheckerByCatch();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
