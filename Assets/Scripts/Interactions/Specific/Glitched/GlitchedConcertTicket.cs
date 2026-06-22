using UnityEngine;

public class GlitchedConcertTicket : GlitchedNote
{
    [SerializeField] private Piano _piano;

    private void Start()
    {
        if (GameState.Instance.ReadConcertTicket)
        {
            _piano.InteractionHitbox.gameObject.SetActive(false);
        }
    }

    protected override void SetGameStateValue()
    {
        _piano.InteractionHitbox.gameObject.SetActive(true);
        GameState.Instance.ReadConcertTicket = true;
    }
}