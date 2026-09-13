using UnityEngine;

public class GlitchedNewspaper : GlitchedNote
{
    protected override bool WasAlreadyRead => GameState.Instance.ReadNewspaper;

    protected override void SetGameStateValue()
    {
        GameState.Instance.ReadNewspaper = true;
    }
}