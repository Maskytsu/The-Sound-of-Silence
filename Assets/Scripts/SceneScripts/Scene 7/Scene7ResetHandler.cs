using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene7ResetHandler : MonoBehaviour
{
    public bool SceneWasReseted { get; private set; }
    public bool SafeRoomReached { get; private set; }
    public bool TookGun { get; private set; }

    [Header("Prefabs")]
    [SerializeField] private Scene7ResetedChecker _checkerPrefab;
    [Header("Scene Objects")]
    [SerializeField] private CatchingPlayerMonsterState _catchingState;
    [SerializeField] private GameState _gameState;
    [SerializeField] private Trigger _playerSafeRoom1Trigger;

    private bool _safeRoomReached;

    private void Awake()
    {
        if (Scene7ResetedChecker.Instance != null)
        {
            SceneWasReseted = true;
            SafeRoomReached = Scene7ResetedChecker.Instance.SafeRoomReached;
            _gameState.TookPills = Scene7ResetedChecker.Instance.TookPills;
            TookGun = Scene7ResetedChecker.Instance.TookGun;

            Destroy(Scene7ResetedChecker.Instance.gameObject);
        }
    }

    private void Start()
    {
        _catchingState.OnPlayerCatched += SpawnChecker;

        Debug.LogWarning("SafeRoomReached doesn't work yet!");
        //_playerSafeRoom1Trigger.OnObjectTriggerEnter += () => SafeRoomReached = true;
    }

    private void SpawnChecker()
    {
        Scene7ResetedChecker checker = Instantiate(_checkerPrefab);

        checker.SafeRoomReached = _safeRoomReached;

        checker.TookPills = _gameState.TookPills;

        if (ItemManager.Instance.HaveGun) checker.TookGun = true;
        else checker.TookGun = false;
    }
}
