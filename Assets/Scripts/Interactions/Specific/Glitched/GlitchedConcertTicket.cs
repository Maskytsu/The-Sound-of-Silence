using NaughtyAttributes;
using UnityEngine;

public class GlitchedConcertTicket : GlitchedNote
{
    [InfoBox("intentionaly left as null on scene 1 to skip this start logic")]
    [SerializeField] private Piano _piano;

    protected override bool WasAlreadyRead => GameState.Instance.ReadNewspaper;

    private void Start()
    {
        if (GameState.Instance.ReadConcertTicket)
        {
            _piano?.InteractionHitbox.gameObject.SetActive(false);
        }
    }

    protected override void SetGameStateValue()
    {
        _piano?.InteractionHitbox.gameObject.SetActive(true);
        GameState.Instance.ReadConcertTicket = true;
    }
}