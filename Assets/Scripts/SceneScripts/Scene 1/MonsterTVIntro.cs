using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MonsterTVIntro : MonoBehaviour
{
    [Header("IntroDialogue")]
    [SerializeField] private DialogueSequenceScriptable _dialogueSequence;
    [SerializeField] private float _fadeSpeed = 3f;
    [SerializeField] private BlackoutBackground _blackoutBackgroundPrefab;
    [Header("Getting up")]
    [SerializeField] private CinemachineVirtualCamera _TVCamera;
    [Header("Standing up")]
    [SerializeField] private float _timeToStandUp;
    [SerializeField] private Crutches _crutches;
    [SerializeField] private Vector3 _playerTargetPos;
    [Header("Next quest")]
    [SerializeField] private QuestScriptable _drinkQuest;

    private BlackoutBackground _blackoutBackground;

    private void Awake()
    {
        _blackoutBackground = Instantiate(_blackoutBackgroundPrefab);
    }

    private void Start()
    {
        UIDisplayManager.Instance.OnHourDisplayEnd += StartDisplayDialogue;
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

        UIDisplayManager.Instance.DisplayDialogueSequence(_dialogueSequence);

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

        InputProvider.Instance.TurnOnPlayerMouseMap();
    }

    private void StartStandUp()
    {
        StartCoroutine(StandUp());
    }

    private IEnumerator StandUp()
    {
        InputProvider.Instance.TurnOffPlayerMouseMap();
        PlayerManager.Instance.PlayerVisuals.SetActive(true);

        Transform player = PlayerManager.Instance.Player.transform;
        Vector3 playerStartingPos = player.position;
        Quaternion playerStartingRot = player.rotation;
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

        InputProvider.Instance.TurnOnPlayerMaps();

        yield return new WaitForSeconds(2f);
        UIDisplayManager.Instance.DisplayNewQuest(_drinkQuest);
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
