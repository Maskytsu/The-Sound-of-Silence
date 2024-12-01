using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{
    [SerializeField] private Transform _movePosition;
    [SerializeField] private NavMeshAgent _monsterAgent;

    private void Start()
    {
        _monsterAgent.isStopped = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _monsterAgent.destination = _movePosition.position;

            _monsterAgent.isStopped = !_monsterAgent.isStopped;
        }
    }
}
