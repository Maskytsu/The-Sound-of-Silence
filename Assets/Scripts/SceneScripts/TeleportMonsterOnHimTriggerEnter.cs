using System.Collections.Generic;
using UnityEngine;

public class TeleportMonsterOnHimTriggerEnter : MonoBehaviour
{
    [SerializeField] private MonsterStateMachine _stateMachine;
    [SerializeField] private TeleportingRandomMonsterState _teleportingState;
    [SerializeField] private List<Trigger> _monsterTriggers;

    private void Start()
    {
        foreach (Trigger trigger in _monsterTriggers)
        {
            trigger.OnObjectTriggerEnter += TeleportMonster;
        }
    }

    private void TeleportMonster()
    {
        _stateMachine.ChangeState(_teleportingState);
    }
}
