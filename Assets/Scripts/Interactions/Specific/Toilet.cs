using DG.Tweening;
using FMODUnity;
using System;
using System.Collections;
using UnityEngine;


public class Toilet : Interactable
{
    public Action OnCoverOpened;

    [Space]
    [SerializeField] private Transform _toiletCover;

    private bool coverOpened = false;
    private bool _inMotion = false;

    protected override void Interact()
    {
        if (!_inMotion)
        {
            HidePrompt();

            if (!coverOpened) StartCoroutine(OpenCover());
            else StartCoroutine(CloseCover());
        }
    }

    private IEnumerator OpenCover()
    {
        _inMotion = true;
        InputProvider.Instance.TurnOffPlayerMaps();

        RuntimeManager.PlayOneShot(FmodEvents.Instance.OpenToiletCover);
        Tween rotationTween = _toiletCover.DOLocalRotate(new Vector3(0, 0, 97f), 2f);

        while (rotationTween.IsPlaying())
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.25f);

        coverOpened = true;
        OnCoverOpened?.Invoke();

        _inMotion = false;
    }

    private IEnumerator CloseCover()
    {
        _inMotion = true;

        RuntimeManager.PlayOneShot(FmodEvents.Instance.Flush);
        yield return new WaitForSeconds(1f);

        RuntimeManager.PlayOneShot(FmodEvents.Instance.CloseToiletCover);
        Tween rotationTween = _toiletCover.DOLocalRotate(new Vector3(0, 0, 0), 2f);

        while (rotationTween.IsPlaying())
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.25f);

        coverOpened = false;
        _interactionHitbox.gameObject.SetActive(false);

        _inMotion = false;
    }
}
