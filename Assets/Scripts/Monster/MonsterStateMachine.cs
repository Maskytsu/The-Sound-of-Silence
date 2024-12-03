using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using NaughtyAttributes;

public class MonsterStateMachine : MonoBehaviour
{
    public MonsterState CurrentState { get; private set; }

    [field: SerializeField] public NavMeshAgent NavMeshAgent { get; private set; }
    [field: SerializeField] public MonsterFieldOfView MonsterFOV { get; private set; }
    [field: SerializeField] public MonsterStateCoroutines StateCoroutines { get; private set; }
    [field: SerializeField] public Transform MonsterTransform { get; private set; }
    [field: SerializeField] public List<Transform> PatrolingPoints { get; private set; }

    private int _currentPointIndex;

    [ShowNativeProperty] private string _currentState => DisplayStateInEditor();

    private void Awake()
    {
        InitializeState();
    }

    private void Update()
    {
        CurrentState.Update();
    }

    public void ChangeState(MonsterState givenState)
    {
        CurrentState.ExitState();
        CurrentState = givenState;
        CurrentState.EnterState();
    }

    public Vector3 RandomDifferentPositionPoint()
    {
        int randomDifferentIndex = Random.Range(0, PatrolingPoints.Count);

        while (randomDifferentIndex == _currentPointIndex)
        {
            randomDifferentIndex = Random.Range(0, PatrolingPoints.Count);
        }

        _currentPointIndex = randomDifferentIndex;
        return PatrolingPoints[_currentPointIndex].position;
    }

    private void InitializeState()
    {
        _currentPointIndex = 0;
        CurrentState = new WalkingMonsterState(this, PatrolingPoints[_currentPointIndex]);
        CurrentState.EnterState();
    }

    private string DisplayStateInEditor()
    {
        if (!Application.isPlaying) return "None";

        string stateName = CurrentState.GetType().ToString();
        stateName = stateName.Substring(0, stateName.Length - 12);
        return stateName;
    }
}