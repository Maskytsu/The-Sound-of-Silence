using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonsterSeenDetector : MonoBehaviour
{
    [InfoBox("REMEMBER - it also works when scene view camera looks at it!")]
    [SerializeField] private Renderer _detectionRenderer;
    [SerializeField] private UnityEvent _monsterUndetected = new();
    [SerializeField] private List<DetectionStage> _monsterDetectionStages = new();
    [Space]
    [ReadOnly, SerializeField] private float _detectionTimer = 0.0f;
    [ReadOnly, SerializeField] private int _currentStageIndex = 0;
    [ReadOnly, SerializeField] private bool _maxStageReached = false;

    private DetectionStage CurrentStage => _monsterDetectionStages[_currentStageIndex];

    private void Update()
    { 
        if (_monsterDetectionStages.Count == 0)
        {
            return;
        }

        if (_detectionRenderer.isVisible)
        {
            _detectionTimer += Time.deltaTime;
        }
        else if (_detectionTimer != 0)
        {
            _detectionTimer = 0.0f;
            _currentStageIndex = 0;
            _monsterUndetected?.Invoke();
            CurrentStage.monsterUndetected?.Invoke();
            _maxStageReached = false;
        }

        if (!_maxStageReached && _detectionTimer >= CurrentStage.detectionDuration)
        {
            if (_currentStageIndex == _monsterDetectionStages.Count - 1)
            {
                CurrentStage.monsterDetected?.Invoke();
                _maxStageReached = true;
            }
            else
            {
                _detectionTimer -= CurrentStage.detectionDuration;
                CurrentStage.monsterDetected?.Invoke();
                _currentStageIndex++;
            }
        }
    }

    [Serializable]
    private class DetectionStage
    {
        [SerializeField] public float detectionDuration = 3.0f;
        [SerializeField] public UnityEvent monsterDetected = new();
        [SerializeField] public UnityEvent monsterUndetected = new();
    }
}