using Cinemachine;
using DG.Tweening;
using DG.Tweening.Core;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MonsterTVIntro : MonoBehaviour
{
    [Header("Intro Dialogue")]
    [SerializeField] private DialogueSequenceScriptable _dialogueSequence;
    [SerializeField] private float _fadingTime = 3f;
    [SerializeField] private BlackoutBackground _blackoutBackgroundPrefab;
    [Header("Getting Up")]
    [SerializeField] private CinemachineVirtualCamera _TVCamera;
    [Header("Standing Up")]
    [SerializeField] private float _timeToStandUp;
    [SerializeField] private GameObject _mouseMovementTutorialPrefab;
    [SerializeField] private Crutches _crutches;
    [SerializeField] private Vector3 _playerTargetPos;
    [Header("Next Quest")]
    [SerializeField] private GameObject _WASDTutorialPrefab;
    [SerializeField] private QuestScriptable _drinkQuest;

    private BlackoutBackground _blackoutBackground;
    private GameObject _mouseMovementTutorial;

    private PlayerInputActions.PlayerMovementMapActions PlayerMovementMap => InputProvider.Instance.PlayerMovementMap;

    private void Awake()
    {
        _blackoutBackground = Instantiate(_blackoutBackgroundPrefab);
    }

    private void Start()
    {
        UIManager.Instance.OnHourDisplayEnd += StartDisplayDialogue;
        _dialogueSequence.OnDialogueEnd += StartGetUp;
        _crutches.OnInteract += StartStandUp;
    }

    private void StartDisplayDialogue()
    {
        StartCoroutine(DisplayDialogue());
    }
    private IEnumerator DisplayDialogue()
    {
        yield return new WaitForSeconds(1f);

        UIManager.Instance.DisplayDialogueSequence(_dialogueSequence);

        yield return new WaitForSeconds(CalculateTimeToOpenEyes());

        _blackoutBackground.Image.DOFade(0f, _fadingTime).OnComplete(() =>
        {
            Destroy(_blackoutBackground.gameObject);
        });
    }

    private void StartGetUp()
    {
        StartCoroutine(GetUp());
    }
    private IEnumerator GetUp()
    {
        yield return new WaitForSeconds(1f);

        PlayerManager.Instance.VirtualMainCamera.enabled = true;
        _TVCamera.enabled = false;

        yield return null;

        while (PlayerManager.Instance.CameraBrain.IsBlending)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        _mouseMovementTutorial = Instantiate(_mouseMovementTutorialPrefab);
        InputProvider.Instance.TurnOnPlayerMainMap();
    }

    private void StartStandUp()
    {
        StartCoroutine(StandUp());
    }
    private IEnumerator StandUp()
    {
        Destroy(_mouseMovementTutorial);
        InputProvider.Instance.TurnOffPlayerMainMap();
        PlayerManager.Instance.PlayerVisuals.SetActive(true);

        Transform player = PlayerManager.Instance.Player.transform;

        Vector3 playerTargetRot = new Vector3(0, player.rotation.eulerAngles.y, 0);

        Tween moveTween = player.DOMove(_playerTargetPos, 2f).SetEase(Ease.InOutSine);
        Tween rotateTween = player.DORotate(playerTargetRot, 2f).SetEase(Ease.InOutSine);

        while (moveTween.IsActive() || rotateTween.IsActive())
        {
            yield return null;
        }

        PlayerManager.Instance.PlayerCharacterController.enabled = true;

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(DisplayWASDTutorial());
        InputProvider.Instance.TurnOnPlayerMaps();

        yield return new WaitForSeconds(2f);
        UIManager.Instance.DisplayNewQuest(_drinkQuest);
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

    private float CalculateTimeToOpenEyes()
    {
        float timeOfDialogues = 0;

        foreach(var dialogueLine in _dialogueSequence.DialogueLines)
        {
            timeOfDialogues += dialogueLine.DisplayTime;
        }

        return 0.5f * timeOfDialogues;
    }
}