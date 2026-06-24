using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MultigletonMonobehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance
    {
        get
        {
            if (_instance != null && _instance.enabled) return _instance;

            var previous = _instance;
            var enabledInstance = _allInstances.FirstOrDefault(i => i != null && i.enabled);
            if (enabledInstance != null) _instance = enabledInstance;

            return _instance;
        }
    }

    private static T _instance;

    private static List<T> _allInstances = new();

    protected virtual void Awake()
    {
        _allInstances.Add(this as T);
    }
}