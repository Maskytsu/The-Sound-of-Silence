using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PianoFeverMap", menuName = "Piano Fever/Audio Map")]
public class PianoFeverMap : ScriptableObject
{
    public AudioClip clip;
    public List<Node> nodes = new();

    public bool playAudioOnNode;
    public AudioClip nodeAudio;
    public float nodeFallingDuration = 2.0f;
    public int pianoLinesCount = 5;

    [Serializable]
    public class Node
    {
        public float timestamp;
        public string name;
    }
}