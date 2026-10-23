using NaughtyAttributes;
using UnityEngine;

public class GlitchedConcertTicket : GlitchedNote
{
    [InfoBox("intentionaly left as null on scene 1 to skip this start logic")]
    [SerializeField] private Piano _piano;

    protected override bool WasAlreadyRead => GameState.Instance.ReadConcertTicket;

    private void Start()
    {
        Debug.Log("SET PIANO TO: " + GameState.Instance.ReadConcertTicket);
        _piano?.InteractionHitbox.gameObject.SetActive(GameState.Instance.ReadConcertTicket);
    }

    protected override void SetGameStateValue()
    {
        Debug.Log("SET PIANO TO: " + true);
        _piano?.InteractionHitbox.gameObject.SetActive(true);
        GameState.Instance.ReadConcertTicket = true;
    }
}