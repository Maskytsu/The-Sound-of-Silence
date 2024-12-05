using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using NaughtyAttributes;
using UnityEditor;

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
    [Space]
    [SerializeField] private CatchingPlayerMonsterState _catchingPlayerState;
    [SerializeField] private float _catchingRange;

    private int _currentPointIndex;

    private void Awake()
    {
        InitializeState();
    }

    private void Update()
    {
        CurrentState.StateUpdate();
        CatchPlayerIfInCatchRange();
    }

    private void OnDrawGizmosSelected()
    {
        DrawCatchingRange();
    }

    public void ChangeState(MonsterState givenState)
    {
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

    private void CatchPlayerIfInCatchRange()
    {
        if (CurrentState == _catchingPlayerState) return;

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
        _currentPointIndex = 0;
        _startingWalkingState.ChosenPosition = _patrolingPoints[_currentPointIndex].position;

        CurrentState = _startingWalkingState;
        CurrentState.gameObject.SetActive(true);
        CurrentState.EnterState();
    }

    private void DrawCatchingRange()
    {
        Handles.color = Color.red;
        Handles.DrawWireArc(MonsterFOV.FOVStartingPoint.position, Vector3.up, Vector3.forward, 360, _catchingRange);
    }
}