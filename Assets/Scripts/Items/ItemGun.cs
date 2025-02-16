using FMODUnity;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGun : Item
{
    [SerializeField] private ParticleSystem _shootParticle;
    [Layer, SerializeField] private int _monsterLayer;

    private bool _ableToShoot = true;
    private float _gunRange = 60f;
    private Transform PlayerCamera => PlayerObjects.Instance.PlayerVirtualCamera.transform;

    public override void UseItem()
    {
        //shoot
        if (_ableToShoot) Shoot();
    }

    private void Shoot()
    {
        StartCoroutine(ShootDelay());
        _shootParticle.Play();
        RuntimeManager.PlayOneShot(FmodEvents.Instance.H_GunShot);

        if (Physics.Raycast(PlayerCamera.position, PlayerCamera.forward, out RaycastHit hit, _gunRange))
        {
            if (hit.transform.gameObject.layer == _monsterLayer)
            {
                MonsterCollider collider = hit.transform.gameObject.GetComponent<MonsterCollider>();
                collider.OnMonsterHit?.Invoke();
            }
        }
    }

    private IEnumerator ShootDelay()
    {
        _ableToShoot = false;
        yield return new WaitForSeconds(1.5f);
        _ableToShoot = true;
    }
}
