using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShortCarDreamAnimation : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private Blackout _blackoutPrefab;
    [SerializeField] private Blackout _whiteBlackoutPrefab;
    [Header("Scene Objects")]
    [SerializeField] private Transform _car;
    [SerializeField] private Transform _carPositionAtDreamEnd;
    [Header("Parameters")]
    [Scene, SerializeField] private string _nextScene;

    private Transform _player;
    private float _wholeDreamSceneDuration = 20f;

    private void Start()
    {
        _player = PlayerManager.Instance.Player.transform;

        StartCoroutine(CarAnimation());
    }

    private void LateUpdate()
    {
        RestrainPlayersRotation();
    }

    private void RestrainPlayersRotation()
    {
        //restrain between -90 and 90 on Y axis
        if (_player.localEulerAngles.y < 270 && _player.localEulerAngles.y > 180)
        {
            _player.localRotation = Quaternion.Euler(0, 270, 0);
        }
        else if (_player.localEulerAngles.y > 90 && _player.localEulerAngles.y < 180)
        {
            _player.localRotation = Quaternion.Euler(0, 90, 0);
        }
    }

    private IEnumerator CarAnimation()
    {
        float startingTime = Time.time;

        _car.DOMoveZ(_carPositionAtDreamEnd.position.z, _wholeDreamSceneDuration).SetEase(Ease.Linear);

        Blackout blackout = Instantiate(_blackoutPrefab);
        Tween fadeTween = blackout.Image.DOFade(0f, 2f);
        while (fadeTween.IsActive())
        {
            yield return null;
        }
        yield return null;
        Destroy(blackout.gameObject);

        float timeBetweenFadings = _wholeDreamSceneDuration - (Time.time - startingTime) - 2f;
        yield return new WaitForSeconds(timeBetweenFadings);

        blackout = Instantiate(_whiteBlackoutPrefab);
        blackout.SetAlphaToZero();
        fadeTween = blackout.Image.DOFade(1f, 1f);
        while (fadeTween.IsActive())
        {
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(_nextScene);
    }
}
