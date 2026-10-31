using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PianoFeverConfig", menuName = "Piano Fever/Config")]
public class PianoFeverConfig : ScriptableObject
{
    [field: SerializeField] public List<PianoFeverTapResultData> TapResults { get; private set; } = new();

    public void GetTapResult(float tapError, out bool isMiss, out PianoFeverTapResultData tapResult)
    {
        foreach (var result in TapResults)
        {
            if (Mathf.Abs(tapError) < result.ErrorTresholdInSeconds)
            {
                isMiss = false;
                tapResult = result;
                return;
            }
        }
        isMiss = true;
        tapResult = null;
    }
}