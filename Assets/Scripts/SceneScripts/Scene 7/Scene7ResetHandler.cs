using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene7ResetHandler : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private Scene7ResetedChecker _checkerPrefab;
    [Header("Scene Objects")]
    [SerializeField] private CatchingPlayerMonsterState _catchingState;
    [Space]
    [ReadOnly] public bool SceneWasReseted;

    private void Awake()
    {
        //can be called on awake because this instance is DontDestroyOnLoad()
        if (Scene7ResetedChecker.Instance != null)
        {
            SceneWasReseted = true;
            Destroy(Scene7ResetedChecker.Instance.gameObject);
        }
        else
        {
            SceneWasReseted = false;
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

    [Button]
    private void ResetSceneForTesting()
    {
        SpawnChecker();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
