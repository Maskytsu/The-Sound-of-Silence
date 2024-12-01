using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;

public class MonsterFieldOfView : MonoBehaviour
{
    public event Action OnPlayerCatch; 

    public float Radius;
    public float CatchRadius;
    [Range(0, 180)] public float Angle;
    public Transform FOVStartingPoint;
    [HorizontalLine]
    public bool SeesPlayer = false;
    public GameObject SeenPlayer;
    [HorizontalLine]
    [SerializeField] private LayerMask _targetMask;
    [SerializeField] private LayerMask _obstacleMask;
    [Space]
    [SerializeField] private bool _playerCatched;

    private void Start()
    {
        StartCoroutine(LookingForPlayer());
    }

    private IEnumerator LookingForPlayer()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        //while (!_playerCatched)
        while (true)
        {
            yield return wait;
            CheckFieldOfView();
        }
    }

    private void CheckFieldOfView()
    {
        Collider[] rangeCheck = Physics.OverlapSphere(FOVStartingPoint.position, Radius, _targetMask);
        {
            if (rangeCheck.Length != 0)
            {
                Transform target = rangeCheck[0].transform;
                Vector3 directionToTarget = target.position - FOVStartingPoint.position;

                if (Vector3.Angle(FOVStartingPoint.forward, directionToTarget) < Angle / 2)
                {
                    float distanceToTarget = Vector3.Distance(FOVStartingPoint.position, target.position);

                    if (!Physics.Raycast(FOVStartingPoint.position, directionToTarget, distanceToTarget, _obstacleMask))
                    {
                        SeesPlayer = true;
                        SeenPlayer = target.gameObject;

                        Vector3 targetPosition = target.position;
                        Vector3 monsterPosition = FOVStartingPoint.position;
                        targetPosition.y = 0f;
                        monsterPosition.y = 0f;

                        if (Vector3.Distance(targetPosition, monsterPosition) < CatchRadius)
                        {
                            if (!_playerCatched) OnPlayerCatch?.Invoke();
                            _playerCatched = true;
                        }
                    }
                    else
                    {
                        UnseePlayer();
                    }
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
