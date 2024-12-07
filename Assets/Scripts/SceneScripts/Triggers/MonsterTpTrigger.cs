using NaughtyAttributes;
using UnityEngine;

public class MonsterTpTrigger : MonoBehaviour
{
    [Layer] public int MonsterLayer;
    [SerializeField] private TeleportingRandomMonsterState _teleportingState;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == MonsterLayer)
        {
            MonsterStateMachine stateMachine = other.gameObject.GetComponent<MonsterStateMachine>();

            stateMachine.ChangeState(_teleportingState);
        }
    }

    private void OnDrawGizmos()
    {
        BoxCollider boxTrigger = GetComponent<BoxCollider>();

        Gizmos.color = Color.red;
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);

        Gizmos.DrawWireCube(boxTrigger.center, boxTrigger.size);
        Gizmos.matrix = oldMatrix;
    }
}
