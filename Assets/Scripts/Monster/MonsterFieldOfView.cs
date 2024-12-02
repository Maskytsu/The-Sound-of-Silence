using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;

public class MonsterFieldOfView : MonoBehaviour
{
    #region Properties (mostly) for editor script
    public float Radius => _radius;
    public float Angle => _angle;
    public Transform FOVStartingPoint => _fovStartingPoint;
    public bool SeesPlayer => _seesPlayer;
    public GameObject SeenPlayerObj => _seenPlayerObj;
    #endregion

    public event Action OnStartSeeingPlayer;
    public event Action OnStopSeeingPlayer;

    [SerializeField] private LayerMask _playerMask;
    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField] private Transform _fovStartingPoint;
    [Space]
    [SerializeField] private float _radius;
    [SerializeField, Range(0, 180)] private float _angle;

    private bool _seesPlayer = false;
    private GameObject _seenPlayerObj;

    private void Start()
    {
        OnStartSeeingPlayer += () => Debug.Log("MonsterFieldOfView: Started seeing player!");
        OnStopSeeingPlayer += () => Debug.Log("MonsterFieldOfView: Stopped seeing player!");

        StartCoroutine(LookingForPlayer());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator LookingForPlayer()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);

            CheckFieldOfView();
        }
    }

    private void CheckFieldOfView()
    {
        Collider[] playerInRangeCheck = Physics.OverlapSphere(_fovStartingPoint.position, _radius, _playerMask);

        if (playerInRangeCheck.Length > 0)
        {
            Transform player = playerInRangeCheck[0].transform;
            Vector3 directionToPlayer = player.position - _fovStartingPoint.position;

            if (Vector3.Angle(_fovStartingPoint.forward, directionToPlayer) < _angle / 2)
            {
                float distanceToPlayer = Vector3.Distance(_fovStartingPoint.position, player.position);

                if (!Physics.Raycast(_fovStartingPoint.position, directionToPlayer, distanceToPlayer, _obstacleMask))
                {
                    SeePlayer(player.gameObject);
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
        else
        {
            UnseePlayer();
        }

    }

    private void SeePlayer(GameObject player)
    {
        if (!_seesPlayer)
        {
            OnStartSeeingPlayer?.Invoke();
            _seesPlayer = true;
            _seenPlayerObj = player;
        }
    }

    private void UnseePlayer()
    {
        if (_seesPlayer)
        {
            OnStopSeeingPlayer?.Invoke();
            _seesPlayer = false;
            _seenPlayerObj = null;
        }
    }
}