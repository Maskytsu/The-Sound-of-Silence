using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene7ResetHandler : MonoBehaviour
{
    [field: ShowNonSerializedField] public bool SceneWasReseted { get; private set; }
    [field: ShowNonSerializedField] public bool SafeRoomReached { get; private set; }
    [field: ShowNonSerializedField] public bool TookGun { get; private set; }

    [Header("Prefabs")]
    [SerializeField] private Scene7ResetedChecker _checkerPrefab;
    [Header("Scene Objects")]
    [SerializeField] private CatchingPlayerMonsterState _catchingState;
    [SerializeField] private GameState _gameState;
    [SerializeField] private Trigger _playerSafeRoom1Trigger;
    [SerializeField] private SafeRoomReachedResetHandler _safeRoomReset;

    private void Awake()
    {
        SafeRoomReached = false;

        if (Scene7ResetedChecker.Instance != null)
        {
            SceneWasReseted = true;
            SafeRoomReached = Scene7ResetedChecker.Instance.SafeRoomReached;
            _gameState.TookPills = Scene7ResetedChecker.Instance.TookPills;
            TookGun = Scene7ResetedChecker.Instance.TookGun;

            Destroy(Scene7ResetedChecker.Instance.gameObject);
        }

        if (SafeRoomReached) _safeRoomReset.PrepareScene();
    }

    private void Start()
    {
        _catchingState.OnPlayerCatched += SpawnChecker;

        Debug.LogWarning("SafeRoomReached doesn't work yet! (it is implemented but doesn't do anything)");
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
