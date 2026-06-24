using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DebugInfoSetup", menuName = "ScriptableObjects/DebugInfoSetup")]
public class DebugInfoDescriptionSetup : ScriptableObject
{
    public List<string> DebugInfo = new();
}