using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoadChecker : SingletonMonobehaviour<DontDestroyOnLoadChecker>
{
    private List<string> _checkerFlags;

    public void AddFlag(string flag)
    {
        _checkerFlags.Add(flag);
    }

    public bool CheckFlag(string flag)
    {
        return _checkerFlags.Contains(flag);
    }

    protected override void Awake()
    {
        DontDestroyOnLoad(gameObject);
        base.Awake();
    }
}
