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
    [SerializeField] private TutorialOverlay _mouseMovementTutorialPrefab;
    [SerializeField] private TutorialOverlay _WASDTutorialPrefab;
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
    [SerializeField] private float _timeToStandUp;

    private TutorialOverlay _mouseMovementTutorial;
    private EventInstance _TVShowMusic;

    private PlayerInputActions.PlayerMovementMapActions PlayerMovementMap => InputProvider.Instance.PlayerMovementMap;
    private bool WasSceneReseted => Scene1ResetHandler.Instance.SceneWasReseted;
    private BlinkEffect Blink => HUD.Instance.Blink;

    private void Start()
    {
        Blink.SetActiveBlackout(true);
        if (WasSceneReseted) Destroy(_TVPilot);
        UIManager.Instance.OnHourDisplayEnd += () => StartCoroutine(StartCutscene());
        _crutches.OnInteract += () => StartCoroutine(StandUp());
    }

    private IEnumerator StartCutscene()
    {
        if (!WasSceneReseted)
        {
            _dialogueSequence.OnDialogueEnd += () => StartCoroutine(GetUp());
            yield return StartCoroutine(DisplayDialogue());
        }

        Blink.PlayBlinkFromBlack(1.0f);

        if (WasSceneReseted) StartCoroutine(GetUp());
    }

    private IEnumerator DisplayDialogue()
    {
        _TVShowMusic = RuntimeManager.CreateInstance(FmodEvents.Instance.TVShowMusic1);
        RuntimeManager.AttachInstanceToGameObject(_TVShowMusic, _TVScreen.transform);
        _TVShowMusic.start();
        _TVShowMusic.release();

        yield return new WaitForSeconds(1f);

        DialogueManager.Instance.DisplayDialogue(_dialogueSequence);

        yield return new WaitForSeconds(0.5f * _dialogueSequence.GetDialogueDuration());
    }

    private IEnumerator GetUp()
    {
        InputProvider.Instance.TurnOnGameplayOverlayMap();

        yield return new WaitForSeconds(2f);

        if (!WasSceneReseted)
        {
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
        }

        RuntimeManager.PlayOneShot(FmodEvents.Instance.CouchGettingUp);
        PlayerObjects.Instance.PlayerVirtualCamera.enabled = true;
        _TVCamera.enabled = false;

        yield return null;

        while (CameraManager.Instance.CameraBrain.IsBlending)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);


        if (!WasSceneReseted) _mouseMovementTutorial = Instantiate(_mouseMovementTutorialPrefab);
        InputProvider.Instance.TurnOnPlayerCameraMap();
    }

    private IEnumerator StandUp()
    {
        if (_mouseMovementTutorial) _mouseMovementTutorial.EndTutorial();
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

        if (!WasSceneReseted) StartCoroutine(DisplayWASDTutorial());
        InputProvider.Instance.TurnOnPlayerMaps();

        yield return new WaitForSeconds(2f);
        QuestManager.Instance.StartQuest(_drinkQuest);
        _glassOfWater.OnInteract += () => QuestManager.Instance.EndQuest(_drinkQuest);
    }

    private IEnumerator DisplayWASDTutorial()
    {
        TutorialOverlay WASDTutorial = Instantiate(_WASDTutorialPrefab);

        while (PlayerMovementMap.Movement.ReadValue<Vector2>() == Vector2.zero)
        {
            yield return null;
        }

        yield return new WaitForSeconds(3f);

        WASDTutorial.EndTutorial();
    }
}