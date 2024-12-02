using DG.Tweening;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class MonsterPlayerCatcher : MonoBehaviour
{
    public event Action OnPlayerCatch;

    [SerializeField] private MonsterFieldOfView _monsterFov;
    [SerializeField] private MonsterController _monsterController;
    [SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private Collider _collider;
    [SerializeField] private Transform _monsterEye;
    [Space]
    [SerializeField] private float _catchRadius;

    private bool _playerCatched = false;

    private bool ShouldCheckCatchRange => _monsterFov.SeesPlayer && !_playerCatched;

    private void Start()
    {
        OnPlayerCatch += () => Debug.Log("MonsterPlayerCatcher: Player catched!");
    }

    private void Update()
    {
        if (ShouldCheckCatchRange) CheckIfPlayerCatched();
    }

    private void OnDrawGizmosSelected()
    {
        DrawRange();
    }

    private void CheckIfPlayerCatched()
    {
        Vector3 playerPosition = _monsterFov.SeenPlayerObj.transform.position;
        Vector3 monsterPosition = transform.position;

        playerPosition.y = 0f;
        monsterPosition.y = 0f;

        if (Vector3.Distance(playerPosition, monsterPosition) < _catchRadius)
        {
            CatchPlayer();
        }
    }

    private void CatchPlayer()
    {
        OnPlayerCatch?.Invoke();
        _playerCatched = true;

        _monsterFov.enabled = false;
        _monsterController.enabled = false;
        _navMeshAgent.enabled = false;
        _collider.enabled = false;

        StartCoroutine(CatchingAnimation());
    }

    private IEnumerator CatchingAnimation()
    {
        InputProvider.Instance.TurnOffPlayerMaps();

        Vector3 newForwardVector = _monsterFov.SeenPlayerObj.transform.position - transform.position;
        newForwardVector = Quaternion.LookRotation(newForwardVector).eulerAngles;
        newForwardVector.x = 0;

        Tween rotateTween = transform.DORotate(newForwardVector, 1f).SetEase(Ease.InOutSine);
        while (rotateTween.IsPlaying()) yield return null;
        yield return null;

        yield return StartCoroutine(CameraManager.Instance.LookAtTargetAnimation(_monsterEye, 2f));
        yield return new WaitForSeconds(0.5f);

        Vector3 playerPosition = _monsterFov.SeenPlayerObj.transform.position;
        Vector3 monsterPosition = transform.position;
        playerPosition.y = 0f;
        monsterPosition.y = 0f;

        float distance = Vector3.Distance(playerPosition, monsterPosition) - 1.1f;
        Vector3 jumpscarePosition = transform.position;
        jumpscarePosition += transform.forward * distance;

        Tween moveTween = transform.DOMove(jumpscarePosition, 0.1f);
        while (moveTween.IsPlaying()) yield return null;
        yield return null;

        Debug.Log("MonsterPlayerCatcher: Scene reset!");
    }

    private void DrawRange()
    {
        Handles.color = Color.red;
        Handles.DrawWireArc(_monsterFov.FOVStartingPoint.position, Vector3.up, Vector3.forward, 360, _catchRadius);
    }
}
