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

    private void Start()
    {
        UIManager.Instance.OnHourDisplayEnd += () => StartCoroutine(GetUp());
    }

    private IEnumerator GetUp()
    {
        InputProvider.Instance.TurnOnGameplayOverlayMap();
        yield return new WaitForSeconds(2f);

        _playerCamera.enabled = true;
        _lyingInBedCamera.enabled = false;
        yield return null;
        while (CameraManager.Instance.CameraBrain.IsBlending) yield return null;

        yield return new WaitForSeconds(1.5f);
        InputProvider.Instance.TurnOffGameplayOverlayMap();

        _blackout.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        GameManager.Instance.LoadSceneAndSaveGameState(_nextScene);
    }
}
