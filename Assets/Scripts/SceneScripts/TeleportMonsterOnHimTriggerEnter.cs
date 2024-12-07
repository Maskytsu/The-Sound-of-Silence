using UnityEngine;

public class TeleportMonsterOnHimTriggerEnter : MonoBehaviour
{
    [SerializeField] private MonsterStateMachine _stateMachine;
    [SerializeField] private TeleportingRandomMonsterState _teleportingState;
    [SerializeField] private Trigger _monsterTrigger;

    private void Start()
    {
        _monsterTrigger.OnObjectTriggerEnter += TeleportMonster;
    }

    private void TeleportMonster()
    {
        _stateMachine.ChangeState(_teleportingState);
    }
}
