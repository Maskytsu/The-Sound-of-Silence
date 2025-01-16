using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene7ResetedChecker : MonoBehaviour
{
    public static Scene7ResetedChecker Instance { get; private set; }

    public bool SafeRoomReached;
    public bool TookPills;
    public bool ReadNewspaper;
    public bool TookGun;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        CreateInstance();
    }

    private void CreateInstance()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one SceneResetedChecker in the scene.");
        }
        Instance = this;
    }
}
