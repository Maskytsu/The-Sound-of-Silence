using DG.Tweening;
using FMODUnity;
using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FullCarDreamAnimation : MonoBehaviour
{
    [Header("Scene Objects")]
    [SerializeField] private GoodOrBadEndingHandler _endingHandler;
    [SerializeField] private Transform _playerCar;
    [SerializeField] private Transform _playerCarPositionAtDreamEnd;
    [SerializeField] private Transform _crushingCar;
    [SerializeField] private Transform _crushingCarPositionAtDreamEnd;

    private float _wholeDreamSceneDuration = 25f;
    private float _blackoutTime = 1f;
    private float _openEyesSpeed = 1f;
    private float _closeEyesSpeed = 3.0f;

    private void Start()
    {
        InputProvider.Instance.TurnOnPlayerCameraMap();
        StartCoroutine(CarAnimation());
    }

    private void OnDestroy()
    {
        DOTween.KillAll();
    }

    private void LateUpdate()
    {
        RestrainPlayersRotation();
    }

    private void RestrainPlayersRotation()
    {
        Transform player = PlayerObjects.Instance.Player.transform;

        //restrain between -90 and 90 on Y axis
        if (player.localEulerAngles.y < 270 && player.localEulerAngles.y > 180)
        {
            player.localRotation = Quaternion.Euler(0, 270, 0);
        }
        else if (player.localEulerAngles.y > 90 && player.localEulerAngles.y < 180)
        {
            player.localRotation = Quaternion.Euler(0, 90, 0);
        }
    }

    private IEnumerator CarAnimation()
    {
        var blink = HUD.Instance.Blink;
        blink.SetActiveFullBlackout(true);

        yield return new WaitForSeconds(_blackoutTime);

        _playerCar.DOMove(_playerCarPositionAtDreamEnd.position, _wholeDreamSceneDuration).SetEase(Ease.Linear);
        _crushingCar.DOMove(_crushingCarPositionAtDreamEnd.position, _wholeDreamSceneDuration).SetEase(Ease.Linear);

        var openEyesDuration = blink.GetOpenEyesDuration(_openEyesSpeed);
        var closeEyesDuration = blink.GetCloseEyesDuration(_closeEyesSpeed);

        blink.PlayOpenEyes(_openEyesSpeed);
        yield return new WaitForSeconds(openEyesDuration);

        InputProvider.Instance.TurnOnGameplayOverlayMap();
        yield return new WaitForSeconds(_wholeDreamSceneDuration - (openEyesDuration + closeEyesDuration));
        InputProvider.Instance.TurnOffGameplayOverlayMap();

        blink.PlayCloseEyes(_closeEyesSpeed);
        yield return new WaitForSeconds(closeEyesDuration);

        RuntimeManager.PlayOneShot(FmodEvents.Instance.CarCrash);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(_endingHandler.NextScene);
    }
}
