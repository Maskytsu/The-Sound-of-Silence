using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{
    [SerializeField] private Transform _movePosition;
    [SerializeField] private NavMeshAgent _monsterAgent;
    [SerializeField] private MonsterFieldOfView _monsterFov;
    [SerializeField] private List<Transform> _monsterTargetPositions;

    private Vector3 _lastSeenPlayerPos;

    private void Start()
    {
        _monsterAgent.isStopped = true;
    }

    private void Update()
    {
        /*
        if (_monsterFov.SeenPlayer != null)
        {
            _lastSeenPlayerPos = _monsterFov.SeenPlayer.transform.position;
            _monsterAgent.destination = _monsterFov.SeenPlayer.transform.position;
        }
        */

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _monsterAgent.destination = _movePosition.position;

            _monsterAgent.isStopped = !_monsterAgent.isStopped;
        }
    }
}