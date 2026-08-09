using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;

public class MonsterFieldOfView : MonoBehaviour
{
    public event Action OnStartSeeingPlayer;
    public event Action OnStopSeeingPlayer;

    public bool SeesPlayer { get; private set; }
    public GameObject SeenPlayerObj { get; private set; }

    [field: SerializeField] public float Radius { get; private set; }
    [field: SerializeField] public float Angle { get; private set; }
    [field: SerializeField] public Transform FOVStartingPoint { get; private set; }
    [field: SerializeField] public Transform MonsterHeadPoint { get; private set; }
    [Space]
    [SerializeField] private LayerMask _playerMask;
    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField] private LayerMask _coverMask;

    private bool IsFlashlightOn => PlayerObjects.Instance.PlayerEquipment.SpawnedItemInHand is ItemFlashlight { IsFlashlightOn: true };
    private bool IsPlayerHidding => PlayerObjects.Instance.PlayerMovement.IsHidding;
    private Transform PlayerHeadPoint => PlayerObjects.Instance.PlayerHeadPoint;

    private void Start()
    {
        SeesPlayer = false;
        StartCoroutine(SearchForPlayer());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator SearchForPlayer()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            CheckFieldOfView();
        }
    }

    private void CheckFieldOfView()
    {
        Collider[] playerInRangeCheck = Physics.OverlapSphere(FOVStartingPoint.position, Radius, _playerMask);

        if (playerInRangeCheck.Length > 0)
        {
            Transform player = playerInRangeCheck[0].transform;
            Vector3 directionToPlayer = player.position - FOVStartingPoint.position;
            //if crouching that point is under the floor so it needs to be a bit higher
            directionToPlayer.y += 0.3f;

            if (Vector3.Angle(FOVStartingPoint.forward, directionToPlayer) < Angle / 2)
            {
                float distanceToPlayer = Vector3.Distance(FOVStartingPoint.position, player.position);

                if (!Physics.Raycast(FOVStartingPoint.position, directionToPlayer, distanceToPlayer, _obstacleMask))
                {
                    directionToPlayer = PlayerHeadPoint.position - MonsterHeadPoint.position;
                    distanceToPlayer = Vector3.Distance(MonsterHeadPoint.position, PlayerHeadPoint.position);

                    if (!Physics.Raycast(MonsterHeadPoint.position, directionToPlayer, distanceToPlayer, _coverMask))
                    {
                        SeePlayer(player.gameObject);
                    }
                }
                else UnseePlayer();
            }
            else UnseePlayer();
        }
        else UnseePlayer();
    }

    private void SeePlayer(GameObject player)
    {
        if ((!IsPlayerHidding || IsFlashlightOn) && !SeesPlayer)
        {
            OnStartSeeingPlayer?.Invoke();
            SeesPlayer = true;
            SeenPlayerObj = player;
        }
    }

    private void UnseePlayer()
    {
        if (SeesPlayer)
        {
            OnStopSeeingPlayer?.Invoke();
            SeesPlayer = false;
            SeenPlayerObj = null;
        }
    }
}