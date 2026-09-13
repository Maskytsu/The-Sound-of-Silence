using Cinemachine;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillsEnding : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _lyingInBedCamera;
    [SerializeField] private CinemachineVirtualCamera _playerCamera;
    [SerializeField] private Blackout _blackout;
    [Scene, SerializeField] private string _nextScene;
    [Header("Scriptable Objects")]
    [SerializeField] private DialogueSequenceScriptable _dialogueSequence1;
    [SerializeField] private DialogueSequenceScriptable _dialogueSequence2;

    private void Start()
    {
        UIManager.Instance.OnHourDisplayEnd += () => StartCoroutine(GetUp());
    }

    private IEnumerator GetUp()
    {
        InputProvider.Instance.TurnOnGameplayOverlayMap();
        yield return new WaitForSeconds(1f);

        DialogueManager.Instance.DisplayDialogue(_dialogueSequence1);
        yield return new WaitForSeconds(_dialogueSequence1.GetDialogueDuration());

        _playerCamera.enabled = true;
        _lyingInBedCamera.enabled = false;
        yield return null;
        while (CameraManager.Instance.CameraBrain.IsBlending) yield return null;

        DialogueManager.Instance.DisplayDialogue(_dialogueSequence2);
        yield return new WaitForSeconds(_dialogueSequence2.GetDialogueDuration());

        yield return new WaitForSeconds(0.5f);
        InputProvider.Instance.TurnOffGameplayOverlayMap();

        _blackout.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        GameManager.Instance.LoadSceneAndSaveGameState(_nextScene);
    }
}
