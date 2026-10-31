using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PianoFeverMainUI : MonoBehaviour
{
    [SerializeField] PianoFeverConfig config;
    [SerializeField] PianoFeverMap map;
    [Space]
    [SerializeField] AudioSource mainAS;
    [SerializeField] TextMeshProUGUI tapCountsText;
    [SerializeField] Transform pianoLinesLayout;
    [Space]
    [SerializeField] PianoFeverLineUI pianoLinePrefab;

    List<PianoFeverLineUI> spawnedPianoLines = new();
    Dictionary<string, int> tapCounts = new();

    public float CurrentTime => mainAS.time;

    void Start()
    {
        BuildTapCounts();
        SpawnPianoLines();
        PrepareNodeData();
        PlayAudio();
    }

    public void UpdateTapCounts(PianoFeverTapResultData tapResult)
    {
        tapCounts[tapResult != null ? tapResult.Name : "X"]++;
        FillTapCountsText();
    }

    void BuildTapCounts()
    {
        foreach (var tapResult in config.TapResults)
        {
            tapCounts.Add(tapResult.Name, 0);
        }
        tapCounts.Add("X", 0);
        FillTapCountsText();
    }

    void SpawnPianoLines()
    {
        //for (int i = 0; i < map.pianoLinesCount; i++)
        for (int i = 0; i < 1; i++)
        {
            var line = Instantiate(pianoLinePrefab, pianoLinesLayout);
            line.Setup(config, map, this);
            spawnedPianoLines.Add(line);
        }
    }

    void PrepareNodeData()
    {
        foreach(var node in map.nodes)
        {
            PianoFeverLineUI randomLine = spawnedPianoLines[Random.Range(0, spawnedPianoLines.Count)];
            randomLine.AssignNode(node);
        }
    }

    void PlayAudio()
    {
        mainAS.clip = map.clip;
        mainAS.Play();
    }

    void FillTapCountsText()
    {
        string text = "";
        foreach (var kvp in tapCounts)
        {
            text += $"{kvp.Key}: {kvp.Value}\n\n";
        }
        tapCountsText.text = text;
    }
}