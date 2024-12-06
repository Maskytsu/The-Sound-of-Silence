using DG.Tweening;
using System.Collections;
using UnityEngine;

public class CatchingPlayerMonsterState : MonsterState
{
    public Vector3? PlayerPosition;

    [SerializeField] private Transform _monsterEye;

    //---------------------------------------------------------------------------------------------------
    private MonsterFieldOfView MonsterFOV => _stateMachine.MonsterFOV;
    private Transform MonsterTransform => _stateMachine.MonsterTransform;
    //---------------------------------------------------------------------------------------------------
    #region Implementing abstract methods
    public override void EnterState()
    {
        if (PlayerPosition == null) PlayerPosition = _stateMachine.MonsterFOV.SeenPlayerObj.transform.position;

        StartCoroutine(CatchingAnimation());
    }

    public override void StateUpdate()
    {
    }

    public override void ExitState()
    {
        PlayerPosition = null;

        StopAllCoroutines();
    }
    #endregion
    //---------------------------------------------------------------------------------------------------

    public IEnumerator CatchingAnimation()
    {
        Vector3 playerPos = PlayerPosition.Value;

        InputProvider.Instance.TurnOffPlayerMaps();

        Vector3 newForwardVector = playerPos - MonsterTransform.position;
        newForwardVector = Quaternion.LookRotation(newForwardVector).eulerAngles;
        newForwardVector.x = 0;

        Tween rotateTween = MonsterTransform.DORotate(newForwardVector, 1f).SetEase(Ease.InOutSine);
        while (rotateTween.IsPlaying()) yield return null;
        yield return null;

        yield return StartCoroutine(CameraManager.Instance.LookAtTargetAnimation(_monsterEye, 2f));
        yield return new WaitForSeconds(0.5f);

        Vector3 playerPosition = playerPos;
        Vector3 monsterPosition = MonsterTransform.position;
        playerPosition.y = 0f;
        monsterPosition.y = 0f;

        float distance = Vector3.Distance(playerPosition, monsterPosition) - 1.1f;
        Vector3 jumpscarePosition = MonsterTransform.position;
        jumpscarePosition += MonsterTransform.forward * distance;
        jumpscarePosition.y = playerPos.y + 0.3f;

        Tween moveTween = MonsterTransform.DOMove(jumpscarePosition, 0.1f);
        while (moveTween.IsPlaying()) yield return null;
        yield return null;

        _stateMachine.OnPlayerCatched?.Invoke();
    }
}