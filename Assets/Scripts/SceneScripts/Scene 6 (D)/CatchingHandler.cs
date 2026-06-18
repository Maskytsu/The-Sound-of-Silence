using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CatchingHandler : MonoBehaviour
{
    [SerializeField] private Blackout _blackoutPrefab;
    [SerializeField] private CatchingPlayerMonsterState _catchingState;

    private float _fadingTime = 0.75f;
    private float _blackoutTime = 0.5f;

    private SingleSafeRoomHandler _currentCheckpoint;
    private List<SingleSafeRoomHandler> _safeRooms;

    private void Awake()
    {
        _safeRooms = FindObjectsByType<SingleSafeRoomHandler>(FindObjectsSortMode.None).ToList();
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

    private void HandleCheckpointReached(SingleSafeRoomHandler reachedCheckpoint)
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

        Blackout blackout = Instantiate(_blackoutPrefab);
        blackout.SetAlphaToZero();
        Tween fadeTween = blackout.Image.DOFade(1f, _fadingTime);

        while (fadeTween.IsActive()) yield return null;
        yield return new WaitForSeconds(_blackoutTime);

        if (_currentCheckpoint == null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            yield break;
        }

        _currentCheckpoint.ResetToThisCheckpoint();

        fadeTween = blackout.Image.DOFade(0f, _fadingTime);
        while (fadeTween.IsActive()) yield return null;
        yield return new WaitForSeconds(_blackoutTime);

        Destroy(blackout.gameObject);

        InputProvider.Instance.TurnOnGameplayMaps();
    }
}
