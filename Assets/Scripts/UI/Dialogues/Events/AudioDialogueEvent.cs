using FMODUnity;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(AudioDialogueEvent), menuName = "ScriptableObjects/DialogueEvents/" + nameof(AudioDialogueEvent))]
public class AudioDialogueEvent : DialogueEvent
{
    public EventReference EventRef;

    [SerializeField] private bool _overrideDuration;
    [SerializeField, ShowIf(nameof(_overrideDuration))] private float _customDuration;

    public override float EventDuration => _overrideDuration ? _customDuration : AudioManager.EventLength(EventRef);

    public override void HandleEvent() => RuntimeManager.PlayOneShot(EventRef);
}