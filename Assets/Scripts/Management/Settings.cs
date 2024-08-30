using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Settings : MonoBehaviour
{
    public bool fullscreen = true;
    public float brightness = 1f;
    public float volume = 1f;

    public static Settings instance { get; private set; }
    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Found more than one Settings in the scene.");
        }
        instance = this;
    }
}
