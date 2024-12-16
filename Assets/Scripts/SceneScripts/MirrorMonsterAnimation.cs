using DG.Tweening;
using FMODUnity;
using System.Collections;
using UnityEngine;

public class MirrorMonsterAnimation : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject _sharonModelPrefab;
    [Header("Scene Objects")]
    [SerializeField] private Trigger _animationTrigger;
    [SerializeField] private Camera _mirrorCamera;
    [SerializeField] private CanvasGroup _mirrorEffect;
    [SerializeField] private GameObject _monster;
    [SerializeField] private LightSwitch _toiletSwitch;
    [SerializeField] private GameObject _flashlight;
    [SerializeField] private GameObject _phone;
    [SerializeField] private StormEffect _storm;
    [SerializeField] private Door _door;
    [SerializeField] private Pills _pills;
    [Header("Parameters")]
    [SerializeField] private PlayerTargetTransform _mirrorPTT;
    [SerializeField] private PlayerTargetTransform _mirrorClosePTT;
    [SerializeField] private EventReference _monsterSoundNoHearing;
    [SerializeField] private EventReference _monsterSoundHearing;

    private InputProvider InputProvider => InputProvider.Instance;
    private PlayerMovement PlayerMovement => PlayerObjects.Instance.PlayerMovement;
    private PlayerEquipment PlayerEquipment => PlayerObjects.Instance.PlayerEquipment;

    private void Start()
    {
        if (GameState.Instance.TookPills) _animationTrigger.gameObject.SetActive(false);
        else _animationTrigger.gameObject.SetActive(true);

        _animationTrigger.OnObjectTriggerEnter += () => StartCoroutine(MirrorAnimation());
        _pills.OnInteract += UnlockDoor;
    }

    private IEnumerator MirrorAnimation()
    {
        _animationTrigger.gameObject.SetActive(false);
        InputProvider.TurnOffPlayerMaps();
        _monster.SetActive(true);
        GameObject sharonModel = Instantiate(_sharonModelPrefab, PlayerObjects.Instance.Player.transform);
        sharonModel.transform.localPosition = Vector3.zero;
        sharonModel.transform.localRotation = Quaternion.identity;
        PlayerEquipment.ChangeItem(ItemType.NONE);
        float comingTowardsDuration = 2f;
        //-----------------------------------------------------------------------------------------------------------
        yield return StartCoroutine(PlayerMovement.SetTransformAnimation(_mirrorPTT, comingTowardsDuration));
        PlayerMovement.SetCharacterController(false);
        float fadeDuration = 1.5f;
        _mirrorCamera.gameObject.SetActive(true);
        float closeUpDuration = 1f;
        StartCoroutine(PlayerMovement.SetTransformAnimation(_mirrorClosePTT, closeUpDuration));
        //-----------------------------------------------------------------------------------------------------------
        yield return new WaitForSeconds(closeUpDuration - fadeDuration);
        Tween mirrorTween = _mirrorEffect.DOFade(0.9f, fadeDuration);
        //-----------------------------------------------------------------------------------------------------------
        while (mirrorTween.IsPlaying()) yield return null;
        GameObject lightSource = ActivateLightSourceForMirror();
        //-----------------------------------------------------------------------------------------------------------
        yield return new WaitForSeconds(1f);
        AudioManager.Instance.PlayOneShotSpatialized(_monsterSoundHearing, _monster.transform);
        AudioManager.Instance.PlayOneShotSpatialized(_monsterSoundNoHearing, _monster.transform);
        //-----------------------------------------------------------------------------------------------------------
        yield return new WaitForSeconds(3.5f);
        //-----------------------------------------------------------------------------------------------------------
        yield return StartCoroutine(_storm.LightningEffect(0.3f));
        //-----------------------------------------------------------------------------------------------------------
        yield return new WaitForSeconds(2f);
        if (lightSource != null) lightSource.SetActive(false);
        _mirrorEffect.DOFade(0f, fadeDuration);
        //-----------------------------------------------------------------------------------------------------------
        yield return StartCoroutine(PlayerMovement.SetTransformAnimation(_mirrorPTT, closeUpDuration));
        _mirrorCamera.gameObject.SetActive(false);
        Destroy(sharonModel);
        _monster.SetActive(false);
        _door.InteractionHitbox.gameObject.SetActive(false);
        _door.SetOpened(false);
        PlayerMovement.SetCharacterController(true);
        InputProvider.TurnOnPlayerMaps();
    }

    private GameObject ActivateLightSourceForMirror()
    {
        if (GameManager.Instance.IsElectricityOn && _toiletSwitch.IsTurnedOn) return null;

        GameObject lightSource;

        if (ItemManager.Instance.HaveFlashlight) lightSource = _flashlight;
        else lightSource = _phone;

        lightSource.SetActive(true);
        return lightSource;
    }

    private void UnlockDoor()
    {
        _door.InteractionHitbox.gameObject.SetActive(true);
        _door.SetOpened(true);
    }
}
