using UnityEngine;

public class MonsterTVHandler : MonoBehaviour
{
    [SerializeField] private GameObject _TV;

    private OccludedAudioEmitter _tvAmbient;

    private void Start()
    {
        _tvAmbient = AudioManager.PlayOneShotOccludedRI(FmodEvents.Instance.OCC_MonsterTVAmbient, _TV, true, audioOcclusionWidening: 0.85f);
    }

    private void OnDestroy()
    {
        _tvAmbient.EndAudio();
    }
}
