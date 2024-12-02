using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{
    private event Action _onPathEndReached;

    [SerializeField] private NavMeshAgent _monsterAgent;
    [SerializeField] private MonsterFieldOfView _monsterFov;
    [Space]
    [SerializeField] private bool _randomizePositionOrder;
    [SerializeField] private List<Transform> _monsterTargetPositions;

    [ShowNativeProperty] private float RemainingDistance => Application.isPlaying && _monsterAgent.enabled 
        ? _monsterAgent.remainingDistance : Mathf.Infinity;

    private int _positionIndex = 0;
    private Coroutine _waitingCoroutine;

    private void Start()
    {
        MoveToFirstPoint();

        _onPathEndReached += () => _waitingCoroutine = StartCoroutine(WaitForNextPath());
    }

    private void Update()
    {
        CheckIfPathEndWasReached();

        if (_monsterFov.SeesPlayer) ChasePlayer();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void CheckIfPathEndWasReached()
    {
        if (!_monsterAgent.isStopped && RemainingDistance == 0)
        {
            _onPathEndReached?.Invoke();
        }
    }

    private void ChasePlayer()
    {
        if (_waitingCoroutine != null)
        {
            StopCoroutine(_waitingCoroutine);
            _waitingCoroutine = null;
        }

        _monsterAgent.isStopped = false;
        _monsterAgent.SetDestination(_monsterFov.SeenPlayerObj.transform.position);
    }

    private IEnumerator WaitForNextPath()
    {
        if (_waitingCoroutine != null)
        {
            Debug.LogError("Coroutine WaitForNextPatrol is already running! " +
                "\nProbably _onPathReached was called multiple times for some reason.");

            yield break;
        }

        _monsterAgent.isStopped = true;
        Debug.Log("MonsterController: End of path reached!");

        yield return new WaitForSeconds(3f);
        MoveToNextPoint();

        _waitingCoroutine = null;
    }

    private void MoveToNextPoint()
    {
        if (_randomizePositionOrder)
        {
            int randomDifferentIndex = _positionIndex;
            while (randomDifferentIndex != _positionIndex)
            {
                randomDifferentIndex = UnityEngine.Random.Range(0, _monsterTargetPositions.Count);
            }

            _positionIndex = randomDifferentIndex;
        }
        else
        {
            if (_positionIndex < _monsterTargetPositions.Count - 1) _positionIndex++;
            else _positionIndex = 0;
        }

        _monsterAgent.isStopped = false;
        _monsterAgent.SetDestination(_monsterTargetPositions[_positionIndex].position);
    }

    private void MoveToFirstPoint()
    {
        _monsterAgent.isStopped = false;
        _monsterAgent.SetDestination(_monsterTargetPositions[_positionIndex].position);
    }
}