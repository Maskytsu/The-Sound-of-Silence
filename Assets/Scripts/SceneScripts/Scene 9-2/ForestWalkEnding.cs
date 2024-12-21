using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using UnityEngine;

public class ForestWalkEnding : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private Blackout _blackoutPrefab;
    [Header("Scene Objects")]
    [SerializeField] private Trigger _playerTrigger;
    [SerializeField] private PlayerTargetTransform _forestAwayPTT;
    [Header("Parameters")]
    [Scene, SerializeField] private string _nextScene;


    private float _fadingTime = 2f;

    private void Start()
    {
        _playerTrigger.OnObjectTriggerEnter += () => StartCoroutine(ForestWalkAnimation());
    }

    private IEnumerator ForestWalkAnimation()
    {
        InputProvider.Instance.TurnOffPlayerMaps();
        _playerTrigger.gameObject.SetActive(false);


        Transform player = PlayerObjects.Instance.transform;
        Tween movePlayerTween = player.DOMove(_forestAwayPTT.Position, 16f).SetEase(Ease.InOutSine);
        StartCoroutine(PlayerObjects.Instance.PlayerMovement.RotateCharacterAnimation(_forestAwayPTT.Rotation, 2f));

        yield return new WaitForSeconds(10f);

        PlayerObjects.Instance.PlayerEquipment.ChangeItem(ItemType.NONE);


        InputProvider.Instance.TurnOffGameplayOverlayMap();

        Blackout blackoutBackground = Instantiate(_blackoutPrefab);
        blackoutBackground.SetAlphaToZero();
        Tween fadeTween = blackoutBackground.Image.DOFade(1, _fadingTime);
        while (fadeTween.IsActive()) yield return null;

        yield return new WaitForSeconds(2f);
        GameManager.Instance.LoadSceneAndSaveGameState(_nextScene);
    }
}
