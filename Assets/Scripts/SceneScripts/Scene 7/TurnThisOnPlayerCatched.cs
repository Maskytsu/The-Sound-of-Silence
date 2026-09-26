using UnityEngine;

public class TurnThisOnPlayerCatched : MonoBehaviour
{
    void Start()
    {
        PlayerObjects.Instance.PlayerCatchedHandler.OnPlayerCatched += () => gameObject.SetActive(true);
    }
}