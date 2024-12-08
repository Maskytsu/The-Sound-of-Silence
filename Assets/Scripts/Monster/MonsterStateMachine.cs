using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using NaughtyAttributes;
using UnityEditor;
using System;

public class MonsterStateMachine : MonoBehaviour
{
    [ShowNativeProperty] public MonsterState CurrentState { get; private set; }

    [field: SerializeField] public MonsterFieldOfView MonsterFOV { get; private set; }
    [field: SerializeField] public NavMeshAgent Agent { get; private set; }
    [field: SerializeField] public Transform MonsterTransform { get; private set; }
    [Space]
    [SerializeField] private List<Transform> _patrolingPoints;
    [Space]
    [SerializeField] private CatchingPlayerMonsterState _catchingPlayerState;
    [SerializeField] private float _catchingRange;
    [Space]
    [SerializeField] private MonsterState _startingState;

    private int _currentPointIndex;
    private bool _changingStateDisabled = false;
     
    private void Awake()
    {
        InitializeState();
    }

    private void Update()
    {
        CurrentState.StateUpdate();
        CatchPlayerIfInCatchRange();
    }

    private void OnDrawGizmos()
    {
        DrawCatchingRange();
    }

    public void DisableChangingStates()
    {
        _changingStateDisabled = true;
    }

    public void ChangeState(MonsterState givenState)
    {
        if (_changingStateDisabled)
        {
            Debug.LogWarning("Trying to change state but this option is disabled!");
            return;
        }

        if (CurrentState == givenState)
        {
            Debug.LogWarning("Trying to change state to the one that is already active!");
            return;
        }

        CurrentState.ExitState();
        CurrentState.gameObject.SetActive(false);

        CurrentState = givenState;

        CurrentState.gameObject.SetActive(true);
        CurrentState.EnterState();
    }

    public void ChangePatrolingPoints(List<Transform> newPatrolingPoints)
    {
        _currentPointIndex = -1;
        _patrolingPoints = newPatrolingPoints;
    }

    public Vector3 RandomDifferentPositionPoint()
    {
        int randomDifferentIndex = UnityEngine.Random.Range(0, _patrolingPoints.Count);

        while (randomDifferentIndex == _currentPointIndex)
        {
            randomDifferentIndex = UnityEngine.Random.Range(0, _patrolingPoints.Count);
        }

        _currentPointIndex = randomDifferentIndex;
        return _patrolingPoints[_currentPointIndex].position;
    }

    public Vector3 GetPositionFromPointsList(int index)
    {
        _currentPointIndex = index;
        return _patrolingPoints[_currentPointIndex].position;
    }

    private void CatchPlayerIfInCatchRange()
    {
        if (CurrentState == _catchingPlayerState || PlayerObjects.Instance.PlayerMovement.IsHidding) return;

        Vector3 playerPosition = PlayerObjects.Instance.Player.transform.position;
        Vector3 monsterPosition = MonsterTransform.position;

        playerPosition.y = 0f;
        monsterPosition.y = 0f;


        if (Vector3.Distance(playerPosition, monsterPosition) < _catchingRange)
        {
            _catchingPlayerState.PlayerPosition = PlayerObjects.Instance.Player.transform.position;
            ChangeState(_catchingPlayerState);
        }
    }

    private void InitializeState()
    {
        CurrentState = _startingState;
        CurrentState.gameObject.SetActive(true);
        CurrentState.EnterState();
    }

    private void DrawCatchingRange()
    {
        Handles.color = Color.red;
        Handles.DrawWireArc(MonsterFOV.FOVStartingPoint.position, Vector3.up, Vector3.forward, 360, _catchingRange);
    }
}