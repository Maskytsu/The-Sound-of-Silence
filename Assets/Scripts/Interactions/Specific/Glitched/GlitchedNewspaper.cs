using UnityEngine;

public class GlitchedNewspaper : GlitchedNote
{
    protected override void SetGameStateValue()
    {
        GameState.Instance.ReadNewspaper = true;
    }
}