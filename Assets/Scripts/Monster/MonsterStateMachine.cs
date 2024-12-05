using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using NaughtyAttributes;

public class MonsterStateMachine : MonoBehaviour
{
    [ShowNativeProperty] public MonsterState CurrentState { get; private set; }

    [field: SerializeField] public MonsterFieldOfView MonsterFOV { get; private set; }
    [field: SerializeField] public NavMeshAgent Agent { get; private set; }
    [field: SerializeField] public Transform MonsterTransform { get; private set; }
    [Space]
    [SerializeField] private List<Transform> _patrolingPoints;
    [Space]
    [SerializeField] private WalkingMonsterState _startingWalkingState;

    private int _currentPointIndex;

    private void Awake()
    {
        InitializeState();
    }

    private void Update()
    {
        CurrentState.StateUpdate();
    }

    public void ChangeState(MonsterState givenState)
    {
        CurrentState.ExitState();
        CurrentState.gameObject.SetActive(false);

        CurrentState = givenState;

        CurrentState.gameObject.SetActive(true);
        CurrentState.EnterState();
    }

    public Vector3 RandomDifferentPositionPoint()
    {
        int randomDifferentIndex = Random.Range(0, _patrolingPoints.Count);

        while (randomDifferentIndex == _currentPointIndex)
        {
            randomDifferentIndex = Random.Range(0, _patrolingPoints.Count);
        }

        _currentPointIndex = randomDifferentIndex;
        return _patrolingPoints[_currentPointIndex].position;
    }

    private void InitializeState()
    {
        _currentPointIndex = 0;
        _startingWalkingState.ChosenPosition = _patrolingPoints[_currentPointIndex].position;

        CurrentState = _startingWalkingState;
        CurrentState.gameObject.SetActive(true);
        CurrentState.EnterState();
    }
}