using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using NaughtyAttributes;
using UnityEditor;
using System;
using DG.Tweening;
using FMOD.Studio;

public class MonsterStateMachine : MonoBehaviour
{
    public event Action OnMonsterKilled;

    [ShowNativeProperty] public MonsterState CurrentState { get; private set; }

    [field: SerializeField] public MonsterFieldOfView MonsterFOV { get; private set; }
    [field: SerializeField] public NavMeshAgent Agent { get; private set; }
    [field: SerializeField] public Transform MonsterTransform { get; private set; }
    [Space]
    [SerializeField] private List<Transform> _patrolingPoints;
    [Space]
    [Header("States selected from machine")]
    [SerializeField] private CatchingPlayerMonsterState _catchingPlayerState;
    [SerializeField] private float _catchingRange;
    [Space]
    [SerializeField] private OnHitChasingPlayerMonsterState _onHitChasingPlayerState;
    [SerializeField] private MonsterCollider _monsterCollider;
    [SerializeField] private MeshRenderer _monsterHead;
    [Space]
    [SerializeField] private PerishingMonsterState _perishingState;
    [Space]
    [SerializeField] private MonsterState _startingState;
    [Space]
    [Header("Only for testing")]
    [SerializeField] private bool _turnDownMonsterSpeed = false;

    private EventInstance _ambientEventInstance;
    private int _currentPointIndex;
    private bool _changingStateDisabled = false;
    private int _monsterHP = 3;
     
    private void Awake()
    {
        InitializeState();

        _ambientEventInstance = AudioManager.Instance.PlayOneShotOccludedRI(
            FmodEvents.Instance.OCC_MonsterAmbient, transform);
    }

    private void Start()
    {
        _monsterCollider.OnMonsterHit += HitMonster;
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

    private void OnDisable()
    {
        _ambientEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    private void OnDestroy()
    {
        _ambientEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
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

        if (_turnDownMonsterSpeed) Agent.speed = 0.0001f;
    }

    public void ChangePatrolingPoints(List<Transform> newPatrolingPoints)
    {
        _currentPointIndex = -1;
        _patrolingPoints = newPatrolingPoints;
    }

    public Vector3 RandomDifferentPositionPoint()
    {
        if (_patrolingPoints.Count == 0)
        {
            Debug.LogError("Patroling Points are not set!");
            return Vector3.zero;
        }

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

        if (Math.Abs(playerPosition.y - monsterPosition.y) > 2f) return;

        playerPosition.y = 0f;
        monsterPosition.y = 0f;


        if (Vector3.Distance(playerPosition, monsterPosition) < _catchingRange)
        {
            ChangeState(_catchingPlayerState);
        }
    }

    private void HitMonster()
    {
        if (_monsterHP > 0)
        {
            AudioManager.Instance.PlayOneShotOccludedRI(FmodEvents.Instance.OCC_MonsterHit, MonsterTransform);

            Color savedColor = _monsterHead.material.color;
            Sequence flashSequence =  DOTween.Sequence();

            flashSequence.Append(_monsterHead.material.DOColor(Color.white, 0.2f));
            flashSequence.AppendInterval(0.1f);
            flashSequence.Append(_monsterHead.material.DOColor(savedColor, 0.2f));

            _monsterHP--;
            Debug.Log("Monster HP: " + _monsterHP + "/3");

            if (_monsterHP == 0)
            {
                OnMonsterKilled?.Invoke();
                ChangeState(_perishingState);
            }
            else if (CurrentState != _onHitChasingPlayerState)
            {
                ChangeState(_onHitChasingPlayerState);
            }
        }
    }

    private void InitializeState()
    {
        CurrentState = _startingState;
        CurrentState.gameObject.SetActive(true);
        CurrentState.EnterState();

        if (_turnDownMonsterSpeed) Agent.speed = 0.0001f;
    }

    private void DrawCatchingRange()
    {
        Handles.color = Color.red;
        Handles.DrawWireArc(MonsterFOV.FOVStartingPoint.position, Vector3.up, Vector3.forward, 360, _catchingRange);
    }

    [Button]
    private void KillMonster()
    {
        OnMonsterKilled?.Invoke();
        ChangeState(_perishingState);
    }
}