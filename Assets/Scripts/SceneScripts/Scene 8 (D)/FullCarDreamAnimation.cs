using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FullCarDreamAnimation : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private Blackout _blackoutPrefab;
    [Header("Scene Objects")]
    [SerializeField] private GoodOrBadEndingHandler _endingHandler;
    [SerializeField] private Transform _playerCar;
    [SerializeField] private Transform _playerCarPositionAtDreamEnd;
    [SerializeField] private Transform _crushingCar;
    [SerializeField] private Transform _crushingCarPositionAtDreamEnd;

    private float _wholeDreamSceneDuration = 25f;
    private float _blackoutTime = 1f;
    private float _fadingTime = 1.5f;

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
        float startingTime = Time.time;

        Blackout blackout = Instantiate(_blackoutPrefab);
        yield return new WaitForSeconds(_blackoutTime);
        _playerCar.DOMoveZ(_playerCarPositionAtDreamEnd.position.z, _wholeDreamSceneDuration).SetEase(Ease.Linear);
        _crushingCar.DOMoveX(_crushingCarPositionAtDreamEnd.position.x, _wholeDreamSceneDuration).SetEase(Ease.Linear);

        Tween fadeTween = blackout.Image.DOFade(0f, _fadingTime);
        while (fadeTween.IsActive()) yield return null;
        yield return null;
        Destroy(blackout.gameObject);
        InputProvider.Instance.TurnOnGameplayOverlayMap();

        float timeBetweenFadings = _wholeDreamSceneDuration - (Time.time - startingTime) - 1f;
        yield return new WaitForSeconds(timeBetweenFadings);

        InputProvider.Instance.TurnOffGameplayOverlayMap();
        Blackout blackout2 = Instantiate(_blackoutPrefab);
        blackout2.SetAlphaToZero();
        Tween fadeTween2 = blackout2.Image.DOFade(1f, 1f);
        while (fadeTween2.IsActive()) yield return null;

        //crush sound
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(_endingHandler.NextScene);
    }
}
