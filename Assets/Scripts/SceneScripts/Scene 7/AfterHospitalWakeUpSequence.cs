using Cinemachine;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterHospitalWakeUpSequence : MonoBehaviour
{
    public event Action OnAnimationEnd;

    [Header("Scriptable Objects")]
    [SerializeField] private DialogueSequenceScriptable _hearingAidNoKeysDialogue;
    [SerializeField] private DialogueSequenceScriptable _hearingAidTookKeysDialogue;
    [Header("Scene Objects")]
    [SerializeField] private CinemachineVirtualCamera _lyingInBedCamera;
    [SerializeField] private CinemachineVirtualCamera _fastGetUpCamera;
    [SerializeField] private Crutches _crutches;
    [SerializeField] private PlayerTargetTransform _standingPTT;
    [SerializeField] private Scene7ResetHandler _sceneResetHandler;
    [SerializeField] private GameObject _monster;
    [SerializeField] private List<Renderer> _monsterRenderers;

    private void Start()
    {
        if (_sceneResetHandler.SceneWasReseted) Destroy(_monster);

        StartCoroutine(FastGetUp());

        UIManager.Instance.OnHourDisplayEnd += InputProvider.Instance.TurnOnGameplayOverlayMap;

        _crutches.OnInteract += () => StartCoroutine(StandUp());
    }

    private IEnumerator FastGetUp()
    {
        _lyingInBedCamera.enabled = true;

        //time of display hour
        yield return new WaitForSeconds(4.35f);
        if (!_sceneResetHandler.SceneWasReseted) FadeOutMonster(1.5f);

        _fastGetUpCamera.enabled = true;
        _lyingInBedCamera.enabled = false;
        yield return null;

        while (CameraManager.Instance.CameraBrain.IsBlending) yield return null;

        if (!_sceneResetHandler.SceneWasReseted) yield return new WaitForSeconds(2.5f);
        else yield return new WaitForSeconds(1.5f);

        PlayerObjects.Instance.PlayerVirtualCamera.enabled = true;
        _fastGetUpCamera.enabled = false;
        yield return null;

        while (CameraManager.Instance.CameraBrain.IsBlending) yield return null;
        yield return new WaitForSeconds(0.2f);

        DialogueSequenceScriptable dialogue;


        if (!_sceneResetHandler.SceneWasReseted)
        {
            Destroy(_monster);

            if (GameState.Instance.TookKeys) dialogue = _hearingAidTookKeysDialogue;
            else dialogue = _hearingAidNoKeysDialogue;

            dialogue.OnDialogueEnd += InputProvider.Instance.TurnOnPlayerCameraMap;
            UIManager.Instance.DisplayDialogueSequence(dialogue);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            InputProvider.Instance.TurnOnPlayerCameraMap();
        }
    }

    private IEnumerator StandUp()
    {
        InputProvider.Instance.TurnOffPlayerCameraMap();

        yield return new WaitForSeconds(0.5f);
        PlayerObjects.Instance.Player.transform.DOMove(_standingPTT.Position, 1.5f).SetEase(Ease.InOutSine);

        yield return StartCoroutine(PlayerObjects.Instance.PlayerMovement.RotateCharacterAnimation(_standingPTT.Rotation, 2f));
        PlayerObjects.Instance.PlayerMovement.SetCharacterController(true);

        yield return new WaitForSeconds(0.5f);
        InputProvider.Instance.TurnOnPlayerMaps();
        yield return new WaitForSeconds(2f);
        OnAnimationEnd?.Invoke();
    }

    private void FadeOutMonster(float fadingTime)
    {
        foreach (Renderer renderer in _monsterRenderers)
        {
            foreach (Material material in renderer.materials)
            {
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.SetInt("_Surface", 1);
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                material.SetShaderPassEnabled("DepthOnly", false);
                material.SetShaderPassEnabled("SHADOWCASTER", false);
                material.SetOverrideTag("RenderType", "Transparent");
                material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                material.EnableKeyword("_ALPHAPREMULTIPLY_ON");

                material.DOFade(0f, fadingTime);
            }
        }
    }
}