using Cinemachine;
using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using NaughtyAttributes;
using System.Collections;
using UnityEngine;

public class CouchEndingHandler : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private QuestScriptable _couchQuest;
    [SerializeField] private DialogueSequenceScriptable _endingTVDialogue;
    [Header("Scene Objects")]
    [SerializeField] private Couch _couch;
    [SerializeField] private Crutches _crutches;
    [SerializeField] private CinemachineVirtualCamera _puttingOffCamera;
    [SerializeField] private CinemachineVirtualCamera _tvCamera;
    [SerializeField] private GameObject _TVPilot;
    [SerializeField] private MeshRenderer _TVScreen;
    [Header("Parameters")]
    [Scene, SerializeField] private string _nextScene;

    private EventInstance _TVShowMusic;

    private void Start()
    {
        _couchQuest.OnQuestStart += BeginCouchQuest;
    }

    private void BeginCouchQuest()
    {
        _couch.InteractionHitbox.gameObject.SetActive(true);
        _couch.OnInteract += () => StartCoroutine(TVSleepAnimation());
    }

    private IEnumerator TVSleepAnimation()
    {
        InputProvider.Instance.TurnOffPlayerMaps();
        PlayerObjects.Instance.PlayerEquipment.ChangeItem(ItemType.NONE);
        _couch.InteractionHitbox.gameObject.SetActive(false);

        _puttingOffCamera.enabled = true;
        PlayerObjects.Instance.PlayerVirtualCamera.enabled = false;
        yield return null;

        while (CameraManager.Instance.CameraBrain.IsBlending) yield return null;
        yield return new WaitForSeconds(0.5f);

        RuntimeManager.PlayOneShot(FmodEvents.Instance.PuttingOffCrutches);
        _crutches.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);

        _tvCamera.enabled = true;
        _puttingOffCamera.enabled = false;
        yield return null;

        RuntimeManager.PlayOneShot(FmodEvents.Instance.LyingDownOnCouch);
        while (CameraManager.Instance.CameraBrain.IsBlending) yield return null;

        yield return new WaitForSeconds(1f);

        _TVPilot.SetActive(true);

        yield return new WaitForSeconds(1.5f);
        RuntimeManager.PlayOneShot(FmodEvents.Instance.TVPilotClick);
        RuntimeManager.PlayOneShot(FmodEvents.Instance.TVTurnOn);

        _TVShowMusic = RuntimeManager.CreateInstance(FmodEvents.Instance.TVShowBackgroundEnding);
        RuntimeManager.AttachInstanceToGameObject(_TVShowMusic, _TVScreen.gameObject);
        _TVShowMusic.setParameterByName(FmodEvents.DumpingParam, 0);
        _TVShowMusic.start();
        _TVShowMusic.release();

        Tween fadeTVTween = _TVScreen.material.DOColor(new Color(255, 255, 255), 0.5f);

        yield return new WaitForSeconds(1f);

        DialogueManager.DisplayDialogue(_endingTVDialogue);
        yield return new WaitForSeconds(0.5f * _endingTVDialogue.GetDialogueDuration());
        _endingTVDialogue.OnDialogueEnd += () => StartCoroutine(EndScene());

        InputProvider.Instance.TurnOffGameplayOverlayMap();

        HUD.Instance.Blink.PlayCloseEyes(1.0f);
        while (HUD.Instance.Blink.IsPlaying) yield return null;
        DOTween.To(() => 1f, x => _TVShowMusic.setParameterByName(FmodEvents.DumpingParam, x), 1f, 1f);
    }

    private IEnumerator EndScene()
    {
        yield return new WaitForSeconds(1f);
        GameManager.Instance.LoadSceneAndSaveGameState(_nextScene);
    }

    private void OnDestroy()
    {
        _TVShowMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
