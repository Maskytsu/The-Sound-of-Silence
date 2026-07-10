using DG.Tweening;
using FMODUnity;
using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NoteSceneEndingHandler : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private Blackout _blackoutPrefab;
    [Header("Scene Objects")]
    [SerializeField] private Note _note;
    [SerializeField] private Transform _monsterArms;
    [SerializeField] private Transform _whisperSoundPos;
    [Header("Parameters")]
    [Scene, SerializeField] private string _nextScene;

    private float _blackoutTime = 1f;
    private float _fadingTime = 1.5f;

    private void Start()
    {
        _note.OnFirstReadingEnd += () => StartCoroutine(MonsterArmsAnimation());
    }

    private IEnumerator MonsterArmsAnimation()
    {
        _note.InteractionHitbox.OnUnpointed?.Invoke();
        _note.InteractionHitbox.gameObject.SetActive(false);

        InputProvider.Instance.TurnOffGameplayMaps();

        yield return new WaitForSeconds(0.5f);

        float armsTweenDuration = 1.5f;
        _monsterArms.gameObject.SetActive(true);
        _monsterArms.DOLocalMoveZ(0, armsTweenDuration);

        yield return new WaitForSeconds(armsTweenDuration / 2);

        RuntimeManager.PlayOneShotAttached(FmodEvents.Instance.SPT_MonsterWhisper, _whisperSoundPos.gameObject);

        yield return new WaitForSeconds(0.5f + armsTweenDuration / 2);

        InputProvider.Instance.TurnOffGameplayOverlayMap();

        HUD.Instance.Blink.PlayCloseEyes(3.0f);
        while (HUD.Instance.Blink.IsPlaying) yield return null;

        yield return new WaitForSeconds(_blackoutTime);
        SceneManager.LoadScene(_nextScene);
    }
}
