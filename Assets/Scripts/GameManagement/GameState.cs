using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }

    public bool MessageSentToClaire = false;
    public bool MessageSentToMechanic = false;
    public bool CalledToClaire = false;
    public bool CalledToPolice = false;
    public bool TookPills = false;

    private void Awake()
    {
        CreateInstance();
    }

    private void CreateInstance()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one GameState in the scene.");
        }
        Instance = this;
    }
}
