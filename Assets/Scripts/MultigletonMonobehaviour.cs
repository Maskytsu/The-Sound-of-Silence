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
            _allInstances.RemoveAll(i => i == null);
            if (_allInstances.Count == 0) _allInstances = FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();

            var enabledInstance = _allInstances.FirstOrDefault(i => i != null && i.enabled);
            if (enabledInstance != null) _instance = enabledInstance;
            if (_instance == null) _instance = _allInstances.FirstOrDefault(i => i != null);

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