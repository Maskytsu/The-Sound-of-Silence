using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CatchingPlayerMonsterState : MonsterState
{
    public event Action OnPlayerCatched;

    [SerializeField] private Transform _monsterEye;
    [Space]
    [SerializeField] private Blackout _blackoutPrefab;

    private float _fadingTime = 0.75f;
    private float _blackoutTime = 0.5f;

    //---------------------------------------------------------------------------------------------------
    private Vector3 PlayerPos => PlayerObjects.Instance.Player.transform.position;
    private Transform MonsterTransform => _stateMachine.MonsterTransform;
    //---------------------------------------------------------------------------------------------------
    #region Implementing abstract methods
    public override void EnterState()
    {
        _stateMachine.DisableChangingStates();
        StartCoroutine(CatchingAnimation());
    }

    public override void StateUpdate()
    {
    }

    public override void ExitState()
    {
        StopAllCoroutines();
    }
    #endregion
    //---------------------------------------------------------------------------------------------------

    private IEnumerator CatchingAnimation()
    {
        InputProvider.Instance.TurnOffPlayerMaps();

        Vector3 newForwardVector = PlayerPos - MonsterTransform.position;
        newForwardVector = Quaternion.LookRotation(newForwardVector).eulerAngles;
        newForwardVector.x = 0;

        Tween rotateTween = MonsterTransform.DORotate(newForwardVector, 1f).SetEase(Ease.InOutSine);
        while (rotateTween.IsPlaying()) yield return null;
        yield return null;

        yield return StartCoroutine(CameraManager.Instance.LookAtTargetAnimation(_monsterEye, 2f));
        yield return new WaitForSeconds(0.5f);

        Vector3 playerPosition = PlayerPos;
        Vector3 monsterPosition = MonsterTransform.position;
        playerPosition.y = 0f;
        monsterPosition.y = 0f;

        float distance = Vector3.Distance(playerPosition, monsterPosition) - 1.1f;
        Vector3 jumpscarePosition = MonsterTransform.position;
        jumpscarePosition += MonsterTransform.forward * distance;
        jumpscarePosition.y = PlayerPos.y + 0.3f;

        Tween moveTween = MonsterTransform.DOMove(jumpscarePosition, 0.1f);
        while (moveTween.IsPlaying()) yield return null;

        OnPlayerCatched?.Invoke();

        InputProvider.Instance.TurnOffGameplayOverlayMap();
        Blackout blackout = Instantiate(_blackoutPrefab);
        blackout.SetAlphaToZero();
        Tween fadeTween = blackout.Image.DOFade(1f, _fadingTime);

        while (fadeTween.IsActive()) yield return null;
        yield return new WaitForSeconds(_blackoutTime);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}