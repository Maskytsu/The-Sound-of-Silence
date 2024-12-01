using NaughtyAttributes;
using System.Collections;
using UnityEngine;

public class MonsterFieldOfView : MonoBehaviour
{
    public float Radius;
    public Transform FOVStartingPoint;
    [Range(0, 180)] public float Angle;
    [Space]
    public bool SeesPlayer = false;
    public GameObject SeenPlayer;
    [Space]
    [SerializeField] private LayerMask _targetMask;
    [SerializeField] private LayerMask _obstacleMask;

    private void Start()
    {
        StartCoroutine(LookingForPlayer());
    }

    private IEnumerator LookingForPlayer()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            CheckFieldOfView();
        }
    }

    private void CheckFieldOfView()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(FOVStartingPoint.position, Radius, _targetMask);
        {
            if (rangeChecks.Length != 0)
            {
                Transform target = rangeChecks[0].transform;
                Vector3 directionToTarget = target.position - FOVStartingPoint.position;

                if (Vector3.Angle(FOVStartingPoint.forward, directionToTarget) < Angle / 2)
                {
                    float distanceToTarget = Vector3.Distance(FOVStartingPoint.position, target.position);

                    if (!Physics.Raycast(FOVStartingPoint.position, directionToTarget, distanceToTarget, _obstacleMask))
                    {
                        SeesPlayer = true;
                        SeenPlayer = target.gameObject;
                    }
                    else
                        UnseePlayer();
                }
                else
                {
                    UnseePlayer();
                }
            }
            else if (SeesPlayer)
            {
                UnseePlayer();
            }
        }
    }

    private void UnseePlayer()
    {
        SeesPlayer = false;
        SeenPlayer = null;
    }
}
