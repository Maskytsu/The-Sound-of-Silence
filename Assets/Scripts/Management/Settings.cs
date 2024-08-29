using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Settings : MonoBehaviour
{
    public bool Fullscreen = true;
    public float Brightness = 1f;
    public float Volume = 1f;

    public static Settings Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("Found more than one Settings in the scene.");
        }
        Instance = this;
    }
}
