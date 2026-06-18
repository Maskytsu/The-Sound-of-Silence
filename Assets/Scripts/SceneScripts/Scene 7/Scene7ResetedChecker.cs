using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene7ResetedChecker : SingletonMonobehaviour<Scene7ResetedChecker>
{
    protected override void Awake()
    {
        DontDestroyOnLoad(gameObject);
        base.Awake();
    }
}
