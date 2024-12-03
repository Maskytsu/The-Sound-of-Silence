using DG.Tweening;
using FMODUnity;
using System;
using System.Collections;
using UnityEngine;

public class MonsterStateCoroutines : MonoBehaviour
{
    public event Action OnCoroutineEnd;

    [SerializeField] private Transform _monsterTransform;
    [SerializeField] private MonsterFieldOfView _monsterFOV;
    [SerializeField] private Transform _monsterEye;
    [SerializeField] private EventReference _monsterTPSound;

    private Coroutine _currentCoroutine;

    private void Start()
    {
        OnCoroutineEnd += () => _currentCoroutine = null;
    }

    public Coroutine StartStateCoroutine(IEnumerator coroutine)
    {
        if (_currentCoroutine != null)
        {
            Debug.LogError(_currentCoroutine + " is not ended! Cannot play another coroutine.");
            return null;
        }

        return StartCoroutine(coroutine);
    }

    public IEnumerator CatchingAnimation()
    {
        InputProvider.Instance.TurnOffPlayerMaps();

        Vector3 newForwardVector = _monsterFOV.SeenPlayerObj.transform.position - _monsterTransform.position;
        newForwardVector = Quaternion.LookRotation(newForwardVector).eulerAngles;
        newForwardVector.x = 0;

        Tween rotateTween = _monsterTransform.DORotate(newForwardVector, 1f).SetEase(Ease.InOutSine);
        while (rotateTween.IsPlaying()) yield return null;
        yield return null;

        yield return StartCoroutine(CameraManager.Instance.LookAtTargetAnimation(_monsterEye, 2f));
        yield return new WaitForSeconds(0.5f);

        Vector3 playerPosition = _monsterFOV.SeenPlayerObj.transform.position;
        Vector3 monsterPosition = _monsterTransform.position;
        playerPosition.y = 0f;
        monsterPosition.y = 0f;

        float distance = Vector3.Distance(playerPosition, monsterPosition) - 1.1f;
        Vector3 jumpscarePosition = _monsterTransform.position;
        jumpscarePosition += _monsterTransform.forward * distance;

        Tween moveTween = _monsterTransform.DOMove(jumpscarePosition, 0.1f);
        while (moveTween.IsPlaying()) yield return null;
        yield return null;

        OnCoroutineEnd?.Invoke();
    }

    public IEnumerator Teleport(Vector3 position)
    {
        AudioManager.Instance.PlayOneShotOccluded(_monsterTPSound, _monsterTransform);
        yield return new WaitForSeconds(1.5f);
        _monsterTransform.position = position;
        AudioManager.Instance.PlayOneShotOccluded(_monsterTPSound, _monsterTransform);
        yield return new WaitForSeconds(1.5f);

        OnCoroutineEnd?.Invoke();
    }
}