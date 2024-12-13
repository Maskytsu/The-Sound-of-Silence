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

    private void Awake()
    {
        if (Scene7ResetedChecker.Instance != null)
        {
            SceneWasReseted = true;
            SafeRoomReached = Scene7ResetedChecker.Instance.SafeRoomReached;
            TookGun = Scene7ResetedChecker.Instance.TookGun;
            Destroy(Scene7ResetedChecker.Instance.gameObject);
        }
    }

    private void Start()
    {
        _catchingState.OnPlayerCatched += SpawnChecker;
    }

    private void SpawnChecker()
    {
        Scene7ResetedChecker checker = Instantiate(_checkerPrefab);

        //trzeba tutaj dodaæ sprawdzanie tego potem
        if (true) checker.SafeRoomReached = true;
        else checker.SafeRoomReached = false;

        if (ItemManager.Instance.HaveGun) checker.TookGun = true;
        else checker.TookGun = false;
    }
}
