using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PianoFeverLineUI : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] AudioSource buttonAS;
    [SerializeField] TextMeshProUGUI resultText;
    [Space]
    [SerializeField] Transform nodeSpawnPoint;
    [SerializeField] Transform nodeTargetPosition;
    [Space]
    [SerializeField] PianoFeverNodeUI nodePrefab;

    List<NodeData> _assignedNodeData = new();
    PianoFeverConfig _config;
    PianoFeverMap _map;
    PianoFeverMainUI _mainUI;
    Tween _textFadeTween;

    void Update()
    {
        foreach(var nodeData in _assignedNodeData)
        {
            var distance = Mathf.Abs(_mainUI.CurrentTime - nodeData.spawnTime);
            if (!nodeData.wasSpawned && distance < 0.01)
            {
                var nodeUI = Instantiate(nodePrefab, nodeSpawnPoint);
                nodeUI.Setup(_map.nodeFallingDuration, nodeTargetPosition);
                nodeData.wasSpawned = true;
            }
        }
    }

    public void Setup(PianoFeverConfig config, PianoFeverMap map, PianoFeverMainUI mainUI)
    {
        _config = config;
        _map = map;
        _mainUI = mainUI;

        buttonAS.clip = _map.nodeAudio;
        button.onClick.AddListener(Tap);
    }

    public void AssignNode(PianoFeverMap.Node node)
    {
        var spawnTime = node.timestamp - _map.nodeFallingDuration;
        _assignedNodeData.Add(new(node, spawnTime));
    }

    void Tap()
    {
        buttonAS.Stop();
        buttonAS.Play();
        ShowTapInfo();
    }

    void ShowTapInfo()
    {
        _textFadeTween?.Kill();
        PianoFeverTapResultData tapResult = null;
        resultText.text = "X";

        foreach (var nodeData in _assignedNodeData)
        {
            var error = _mainUI.CurrentTime - nodeData.node.timestamp;
            _config.GetTapResult(error, out var isMiss, out tapResult);
            if (!isMiss)
            {
                resultText.text = tapResult.Score.ToString();
                break;
            }
        }

        _mainUI.UpdateTapCounts(tapResult);
        SetTextAlpha(1);
        resultText.color = new(resultText.color.r, resultText.color.g, resultText.color.b, 1);
        _textFadeTween = DOVirtual.DelayedCall(1.0f, () => SetTextAlpha(0)).SetUpdate(true);

        void SetTextAlpha(float alpha) => resultText.color = new(resultText.color.r, resultText.color.g, resultText.color.b, alpha);
    }

    class NodeData
    {
        public PianoFeverMap.Node node;
        public float spawnTime;
        public bool wasSpawned;

        public NodeData(PianoFeverMap.Node node, float spawnTime, bool wasSpawned = false)
        {
            this.node = node;
            this.spawnTime = spawnTime;
            this.wasSpawned = wasSpawned;
        }
    }
}