using System;
using UnityEngine;

public class PlayerCatchedHandler : MonoBehaviour
{
    public event Action OnPlayerCatched = delegate { };

    public void CatchPlayer()
    {
        OnPlayerCatched?.Invoke();
    }
}
