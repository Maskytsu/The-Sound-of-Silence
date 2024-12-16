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
        if (_stateMachine == null)
        {
            Debug.LogWarning("Monster is null. Was it killed?");
            return;
        }

        _stateMachine.ChangeState(_teleportingState);
    }
}
