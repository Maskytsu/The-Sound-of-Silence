using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MonsterTVIntro : MonoBehaviour
{
    [Header("Intro Dialogue")]
    [SerializeField] private DialogueSequenceScriptable _dialogueSequence;
    [SerializeField] private float _fadeSpeed = 3f;
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
    private GameObject _WASDTutorial;
    private bool _WASDTutorialDestroyed = false;

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

    private void Update()
    {
        ManageWASDTutorial();
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

        float alpha = 1f;
        RawImage blackoutImage = _blackoutBackground.Image;

        while (alpha > 0)
        {
            alpha -= Time.deltaTime / _fadeSpeed;
            blackoutImage.color = new Color(blackoutImage.color.r, blackoutImage.color.g, blackoutImage.color.b, alpha);
            yield return null;
        }

        Destroy(_blackoutBackground.gameObject);
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

        player.GetPositionAndRotation(out Vector3 playerStartingPos, out Quaternion playerStartingRot);
        Quaternion playerTargetRot = Quaternion.Euler(0, player.rotation.eulerAngles.y, 0);

        float elapsedTime = 0f;

        while (elapsedTime < _timeToStandUp)
        {
            elapsedTime += Time.deltaTime;
            player.position = Vector3.Lerp(playerStartingPos, _playerTargetPos, elapsedTime / _timeToStandUp);
            player.rotation = Quaternion.Slerp(playerStartingRot, playerTargetRot, elapsedTime / _timeToStandUp);

            yield return null;
        }

        PlayerManager.Instance.PlayerCharacterController.enabled = true;

        yield return new WaitForSeconds(0.5f);

        _WASDTutorial = Instantiate(_WASDTutorialPrefab);
        InputProvider.Instance.TurnOnPlayerMaps();

        yield return new WaitForSeconds(2f);
        UIManager.Instance.DisplayNewQuest(_drinkQuest);
    }

    private void ManageWASDTutorial()
    {
        if (!_WASDTutorialDestroyed & PlayerMovementMap.Movement.ReadValue<Vector2>() != Vector2.zero)
        {
            _WASDTutorialDestroyed = true;
            StartCoroutine(DestroyWASDTutorialDelayed());
        }
    }

    private IEnumerator DestroyWASDTutorialDelayed()
    {
        yield return new WaitForSeconds(3f);
        Destroy(_WASDTutorial);
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
