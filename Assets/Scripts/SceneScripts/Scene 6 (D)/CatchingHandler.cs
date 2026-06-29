using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CatchingHandler : MonoBehaviour
{
    [SerializeField] private MonsterStateMachine _stateMachine;
    [SerializeField] private CatchingPlayerMonsterState _catchingState;

    private float _blackoutTime = 0.5f;

    private Checkpoint _currentCheckpoint;
    private List<Checkpoint> _safeRooms;

    private BlinkEffect Blink => HUD.Instance.Blink;

    private void Awake()
    {
        _safeRooms = FindObjectsByType<Checkpoint>(FindObjectsSortMode.None).ToList();
        _catchingState.OnPlayerCatched += HandlePlayerCatched;

        foreach(var room in _safeRooms)
        {
            room.OnCheckpointReached += HandleCheckpointReached;
        }
    }

    private void OnDestroy()
    {
        _catchingState.OnPlayerCatched -= HandlePlayerCatched;

        foreach (var room in _safeRooms)
        {
            room.OnCheckpointReached -= HandleCheckpointReached;
        }
    }

    private void HandleCheckpointReached(Checkpoint reachedCheckpoint)
    {
        _currentCheckpoint = reachedCheckpoint;
    }

    private void HandlePlayerCatched()
    {
        StartCoroutine(ResetAnimation());
    }

    private IEnumerator ResetAnimation()
    {
        InputProvider.Instance.TurnOffGameplayOverlayMap();

        Blink.PlayBlinkToBlack();

        yield return null;
        while (Blink.IsPlaying) yield return null;
        yield return new WaitForSeconds(_blackoutTime);

        if (_currentCheckpoint == null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            yield break;
        }

        _stateMachine.EnableChangingStates();
        _currentCheckpoint.ResetToThisCheckpoint();

        Blink.PlayBlinkFromBlack();
        yield return null;
        while (Blink.IsPlaying) yield return null;
        yield return new WaitForSeconds(_blackoutTime);

        InputProvider.Instance.TurnOnGameplayMaps();
    }
}
