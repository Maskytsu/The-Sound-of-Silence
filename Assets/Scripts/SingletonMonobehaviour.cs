using UnityEngine;

public class SingletonMonobehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        CreateInstance();
    }

    private void CreateInstance()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError($"Found more than one {typeof(T)} in the scene.");
        }
        Instance = this as T;
    }
}
