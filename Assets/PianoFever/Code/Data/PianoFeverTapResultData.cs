using System;
using UnityEngine;

[Serializable]
public class PianoFeverTapResultData
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public float ErrorTresholdInSeconds { get; private set; }
    [field: SerializeField] public int Score { get; private set; }
}