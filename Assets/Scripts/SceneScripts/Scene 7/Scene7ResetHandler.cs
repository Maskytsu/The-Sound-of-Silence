using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene7ResetHandler : MonoBehaviour
{
    [SerializeField] private bool _autoFakeResetForTesting = false;
    [EnableIf(nameof(_autoFakeResetForTesting))] public bool SceneWasReseted;
    [EnableIf(nameof(_autoFakeResetForTesting))] public bool SafeRoomReached;
    [EnableIf(nameof(_autoFakeResetForTesting))] public bool TookGun;
    [Space]
    [Header("Prefabs")]
    [SerializeField] private Scene7ResetedChecker _checkerPrefab;
    [Header("Scene Objects")]
    [SerializeField] private CatchingPlayerMonsterState _catchingState;
    [SerializeField] private GameState _gameState;
    [SerializeField] private Trigger _playerSafeRoom1Trigger;
    [SerializeField] private SafeRoomReachedResetHandler _safeRoomReset;

    private void Awake()
    {
        if (!_autoFakeResetForTesting)
        {
            //can be called on awake because this instance is DontDestroyOnLoad()
            if (Scene7ResetedChecker.Instance != null)
            {
                SceneWasReseted = true;
                SafeRoomReached = Scene7ResetedChecker.Instance.SafeRoomReached;
                _gameState.TookPills = Scene7ResetedChecker.Instance.TookPills;
                if (SafeRoomReached) TookGun = Scene7ResetedChecker.Instance.TookGun;
                else TookGun = false;

                Destroy(Scene7ResetedChecker.Instance.gameObject);
            }
            else
            {
                SceneWasReseted = false;
                SafeRoomReached = false;
                TookGun = false;
            }
        }

        if (SafeRoomReached) _safeRoomReset.PrepareScene(this);
    }

    private void Start()
    {
        _catchingState.OnPlayerCatched += SpawnChecker;

        _playerSafeRoom1Trigger.OnObjectTriggerEnter += () => SafeRoomReached = true;
    }

    private void SpawnChecker()
    {
        Scene7ResetedChecker checker = Instantiate(_checkerPrefab);

        checker.SafeRoomReached = SafeRoomReached;

        checker.TookPills = _gameState.TookPills;

        if (TookGun) checker.TookGun = true;
        else checker.TookGun = ItemManager.Instance.HaveGun;
    }

    [Button]
    private void ResetSceneForTesting()
    {
        SpawnChecker();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
