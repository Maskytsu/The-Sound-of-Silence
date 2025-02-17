using Cinemachine;
using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class MonsterTVIntro : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private Blackout _blackoutPrefab;
    [SerializeField] private GameObject _mouseMovementTutorialPrefab;
    [SerializeField] private GameObject _WASDTutorialPrefab;
    [Header("Scriptable Objects")]
    [SerializeField] private DialogueSequenceScriptable _dialogueSequence;
    [SerializeField] private QuestScriptable _drinkQuest;
    [Header("Scene Objects")]
    [SerializeField] private CinemachineVirtualCamera _TVCamera;
    [SerializeField] private GameObject _TVPilot;
    [SerializeField] private MeshRenderer _TVScreen;
    [SerializeField] private Crutches _crutches;
    [SerializeField] private PlayerTargetTransform _standingPTT;
    [SerializeField] private GlassOfWater _glassOfWater;
    [Header("Parameters")]
    [SerializeField] private float _fadingBlackoutTime = 3f;
    [SerializeField] private float _timeToStandUp;

    private Blackout _blackoutBackground;
    private GameObject _mouseMovementTutorial;
    private EventInstance _TVShowMusic;

    private PlayerInputActions.PlayerMovementMapActions PlayerMovementMap => InputProvider.Instance.PlayerMovementMap;

    private void Awake()
    {
        _blackoutBackground = Instantiate(_blackoutPrefab);
    }

    private void Start()
    {
        UIManager.Instance.OnHourDisplayEnd += () => StartCoroutine(DisplayDialogue());
        _dialogueSequence.OnDialogueEnd += () => StartCoroutine(GetUp());
        _dialogueSequence.OnDialogueEnd += InputProvider.Instance.TurnOnGameplayOverlayMap;
        _crutches.OnInteract += () => StartCoroutine(StandUp());
    }

    private IEnumerator DisplayDialogue()
    {
        _TVShowMusic = RuntimeManager.CreateInstance(FmodEvents.Instance.TVShowMusic1);
        RuntimeManager.AttachInstanceToGameObject(_TVShowMusic, _TVScreen.transform);
        _TVShowMusic.start();
        _TVShowMusic.release();

        yield return new WaitForSeconds(1f);

        UIManager.Instance.DisplayDialogueSequence(_dialogueSequence);

        yield return new WaitForSeconds(0.5f * _dialogueSequence.DialogueDuration());

        _blackoutBackground.Image.DOFade(0f, _fadingBlackoutTime).OnComplete(() =>
        {
            Destroy(_blackoutBackground.gameObject);
        });
    }

    private IEnumerator GetUp()
    {
        yield return new WaitForSeconds(2f);

        RuntimeManager.PlayOneShot(FmodEvents.Instance.TVPilotClick);
        _TVShowMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        Tween fadeTVTween = _TVScreen.material.DOColor(new Color(0, 0, 0), 0.5f);
        while (fadeTVTween.IsActive())
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        Destroy(_TVPilot);
        yield return new WaitForSeconds(1f);

        RuntimeManager.PlayOneShot(FmodEvents.Instance.CouchGettingUp);
        PlayerObjects.Instance.PlayerVirtualCamera.enabled = true;
        _TVCamera.enabled = false;

        yield return null;

        while (CameraManager.Instance.CameraBrain.IsBlending)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        _mouseMovementTutorial = Instantiate(_mouseMovementTutorialPrefab);
        InputProvider.Instance.TurnOnPlayerCameraMap();
    }

    private IEnumerator StandUp()
    {
        Destroy(_mouseMovementTutorial);
        InputProvider.Instance.TurnOffPlayerCameraMap();

        RuntimeManager.PlayOneShot(FmodEvents.Instance.StandingUp);
        Transform player = PlayerObjects.Instance.Player.transform;
        Vector3 playerTargetRot = new Vector3(0, player.rotation.eulerAngles.y, 0);

        Tween moveTween = player.DOMove(_standingPTT.Position, 2f).SetEase(Ease.InOutSine);
        Tween rotateTween = player.DORotate(playerTargetRot, 2f).SetEase(Ease.InOutSine);

        while (moveTween.IsActive() || rotateTween.IsActive())
        {
            yield return null;
        }

        PlayerObjects.Instance.PlayerMovement.SetCharacterController(true);

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(DisplayWASDTutorial());
        InputProvider.Instance.TurnOnPlayerMaps();

        yield return new WaitForSeconds(2f);
        QuestManager.Instance.StartQuest(_drinkQuest);
        _glassOfWater.OnInteract += () => QuestManager.Instance.EndQuest(_drinkQuest);
    }

    private IEnumerator DisplayWASDTutorial()
    {
        GameObject WASDTutorial = Instantiate(_WASDTutorialPrefab);

        while (PlayerMovementMap.Movement.ReadValue<Vector2>() == Vector2.zero)
        {
            yield return null;
        }

        yield return new WaitForSeconds(3f);

        Destroy(WASDTutorial);
    }
}