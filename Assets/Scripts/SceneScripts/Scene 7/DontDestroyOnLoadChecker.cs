using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoadChecker : SingletonMonobehaviour<DontDestroyOnLoadChecker>
{
    protected override void Awake()
    {
        DontDestroyOnLoad(gameObject);
        base.Awake();
    }
}
