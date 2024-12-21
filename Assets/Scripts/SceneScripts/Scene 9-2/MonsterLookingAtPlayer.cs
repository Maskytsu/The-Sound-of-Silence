using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterLookingAtPlayer : MonoBehaviour
{
    [SerializeField] private Transform _monsterHead;

    void Update()
    {
        _monsterHead.LookAt(PlayerObjects.Instance.PlayerVirtualCamera.transform);
    }
}
