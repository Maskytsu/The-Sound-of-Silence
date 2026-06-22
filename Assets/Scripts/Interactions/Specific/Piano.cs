using FMODUnity;
using System.Collections;
using UnityEngine;

public class Piano : DialogueInteracion
{
    public InteractionHitbox InteractionHitbox => _interactionHitbox;

    private bool _isPlayingAudio = false;
    private EventReference _pianoSound => FmodEvents.Instance.Piano;

    protected override void Interact()
    {
        if (_isPlayingAudio) return;

        if (GameState.Instance.ReadConcertTicket)
        {
            StartCoroutine(PlayPianoSound());
            return;
        }

        base.Interact();
    }

    private IEnumerator PlayPianoSound()
    {
        _isPlayingAudio = true;
        RuntimeManager.PlayOneShot(_pianoSound);
        yield return new WaitForSeconds(AudioManager.Instance.EventLength(_pianoSound));
        _isPlayingAudio = false;
    }
}