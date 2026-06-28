using System.Collections.Generic;
using UnityEngine;

public class TeleportMonsterOnHimTriggerEnter : MonoBehaviour
{
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
        var monsterSM = MonsterStateMachine.Instance;
        if (monsterSM == null)
        {
            Debug.LogWarning("Monster is null. Was it killed?");
            return;
        }

        monsterSM.ChangeState<TeleportingRandomMonsterState>();
    }
}