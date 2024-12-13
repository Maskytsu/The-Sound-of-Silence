using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene6ResetedChecker : MonoBehaviour
{
    public static Scene6ResetedChecker Instance { get; private set; }

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