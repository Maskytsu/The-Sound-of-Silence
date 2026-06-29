using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShortCarDreamAnimation : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private Blackout _whiteBlackoutPrefab;
    [Header("Scene Objects")]
    [SerializeField] private Transform _car;
    [SerializeField] private Transform _carPositionAtDreamEnd;
    [Header("Parameters")]
    [Scene, SerializeField] private string _nextScene;

    private float _carMovementTime = 20f;
    private float _blackoutTime = 1f;
    private float _fadingInTime = 1.5f;
    private float _fadingOutTime = 1f;

    private Transform Player => PlayerObjects.Instance.Player.transform;

    private void Start()
    {
        InputProvider.Instance.TurnOnPlayerCameraMap();
        StartCoroutine(CarAnimation());
    }

    private void LateUpdate()
    {
        RestrainPlayersRotation();
    }

    private void RestrainPlayersRotation()
    {
        //restrain between -90 and 90 on Y axis
        if (Player.localEulerAngles.y < 270 && Player.localEulerAngles.y > 180)
        {
            Player.localRotation = Quaternion.Euler(0, 270, 0);
        }
        else if (Player.localEulerAngles.y > 90 && Player.localEulerAngles.y < 180)
        {
            Player.localRotation = Quaternion.Euler(0, 90, 0);
        }
    }

    private IEnumerator CarAnimation()
    {
        var blink = HUD.Instance.Blink;
        blink.SetActiveBlackout(true);

        yield return new WaitForSeconds(_blackoutTime);

        _car.DOMove(_carPositionAtDreamEnd.position, _carMovementTime).SetEase(Ease.Linear);

        blink.PlayOpenEyes(1.0f);
        while (blink.IsPlaying) yield return null;

        InputProvider.Instance.TurnOnGameplayOverlayMap();

        float timeBetweenFadings = _carMovementTime - (_fadingInTime + _fadingOutTime);
        yield return new WaitForSeconds(timeBetweenFadings);

        InputProvider.Instance.TurnOffGameplayOverlayMap();
        Blackout whiteBlackout = Instantiate(_whiteBlackoutPrefab);
        whiteBlackout.SetAlphaToZero();
        Tween fadeTween = whiteBlackout.Image.DOFade(1f, _fadingOutTime);
        while (fadeTween.IsActive()) yield return null;

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(_nextScene);
    }
}
